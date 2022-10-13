using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Protocol.Battle;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattleController
    {
        BattleFacades battleFacades;
        float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;
        // 服务器下发的生成队列
        Queue<FrameBattleRoleSpawnResMsg> roleSpawnQueue;
        Queue<FrameBulletSpawnResMsg> bulletSpawnQueue;
        Queue<FrameItemSpawnResMsg> itemSpawnQueue;
        // 服务器下发的物理事件队列
        Queue<FrameBulletHitRoleResMsg> bulletHitRoleQueue;
        Queue<FrameBulletHitWallResMsg> bulletHitWallQueue;
        Queue<FrameBulletLifeOverResMsg> bulletTearDownQueue;
        // 服务器下发的人物状态同步队列
        Queue<BattleRoleStateUpdateMsg> roleStateQueue;
        // 服务器下发的资源拾取队列
        Queue<FrameItemPickResMsg> itemPickQueue;

        bool battleBegin;
        bool hasSpawnScene;

        public BattleController()
        {

            roleSpawnQueue = new Queue<FrameBattleRoleSpawnResMsg>();
            bulletSpawnQueue = new Queue<FrameBulletSpawnResMsg>();
            itemSpawnQueue = new Queue<FrameItemSpawnResMsg>();

            bulletHitRoleQueue = new Queue<FrameBulletHitRoleResMsg>();
            bulletHitWallQueue = new Queue<FrameBulletHitWallResMsg>();
            bulletTearDownQueue = new Queue<FrameBulletLifeOverResMsg>();

            roleStateQueue = new Queue<BattleRoleStateUpdateMsg>();

            itemPickQueue = new Queue<FrameItemPickResMsg>();

            UIEventCenter.WorldRoomEnter += ((host, port) =>
            {
                battleFacades.Network.BattleReqAndRes.ConnBattleServer(host, port);
            });
            NetworkEventCenter.Regist_BattleSerConnectHandler(() =>
            {
                battleBegin = true;
            });
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;

            var battleRqs = battleFacades.Network.BattleReqAndRes;
            battleRqs.RegistRes_HeartBeat(OnHeartbeatRes);

            var roleRqs = battleFacades.Network.RoleReqAndRes;
            roleRqs.RegistRes_BattleRoleSpawn(OnBattleRoleSpawn);
            roleRqs.RegistUpdate_WRole(OnWRoleSync);

            var bulletRqs = battleFacades.Network.BulletReqAndRes;
            bulletRqs.RegistRes_BulletSpawn(OnBulletSpawn);
            bulletRqs.RegistRes_BulletHitRole(OnBulletHitRole);
            bulletRqs.RegistRes_BulletHitWall(OnBulletHitWall);
            bulletRqs.RegistRes_BulletTearDown(OnBulletTearDown);

            var ItemReqAndRes = battleFacades.Network.ItemReqAndRes;
            ItemReqAndRes.RegistRes_ItemSpawn(OnItemSpawn);
            ItemReqAndRes.RegistRes_ItemPickUp(OnItemPickUp);
        }

        public void Tick()
        {
            if (battleBegin && !hasSpawnScene)
            {
                GameFightStart();
                hasSpawnScene = true;
            }

            float deltaTime = UnityEngine.Time.deltaTime;

            // == Server Response Physics 
            Tick_BulletHitWall();
            Tick_BulletHitRole();
            Tick_BulletTearDown();

            // == Server Response
            Tick_RoleStateSync();

            Tick_RoleSpawn();
            Tick_BulletSpawn();
            Tick_ItemAssetsSpawn();

            Tick_ItemPick();

            // == Heartbeat
            // if (hasSpawnScene) battleFacades.Network.BattleReqAndRes.SendReq_HeartBeat();
        }

        #region [Input]

        #endregion

        #region [Tick Server Resonse]

        #region [Role]
        void Tick_RoleStateSync()
        {
            while (roleStateQueue.TryPeek(out var stateMsg))
            {
                roleStateQueue.Dequeue();

                RoleState roleState = (RoleState)stateMsg.roleState;
                float x = stateMsg.x / 10000f;
                float y = stateMsg.y / 10000f;
                float z = stateMsg.z / 10000f;
                float eulerX = stateMsg.eulerX / 10000f;
                float eulerY = stateMsg.eulerY / 10000f;
                float eulerZ = stateMsg.eulerZ / 10000f;
                float moveVelocityX = stateMsg.moveVelocityX / 10000f;
                float moveVelocityY = stateMsg.moveVelocityY / 10000f;
                float moveVelocityZ = stateMsg.moveVelocityZ / 10000f;
                float extraVelocityX = stateMsg.extraVelocityX / 10000f;
                float extraVelocityY = stateMsg.extraVelocityY / 10000f;
                float extraVelocityZ = stateMsg.extraVelocityZ / 10000f;
                float gravityVelocity = stateMsg.gravityVelocity / 10000f;

                Vector3 pos = new Vector3(x, y, z);
                Vector3 eulerAngle = new Vector3(eulerX, eulerY, eulerZ);
                Vector3 moveVelocity = new Vector3(moveVelocityX, moveVelocityY, moveVelocityZ);
                Vector3 extraVelocity = new Vector3(extraVelocityX, extraVelocityY, extraVelocityZ);

                var repo = battleFacades.Repo;
                var roleRepo = repo.RoleRepo;
                var fieldRepo = repo.FiledRepo;
                var roleLogic = battleFacades.Repo.RoleRepo.GetByEntityId(stateMsg.entityId);

                if (roleLogic == null)
                {
                    var wRoleId = stateMsg.entityId;
                    var fieldEntity = fieldRepo.Get(1);
                    var domain = battleFacades.Domain.RoleDomain;

                    var roleRenderer = domain.SpawnRoleRenderer(fieldEntity.Role_Group_Renderer);
                    roleRenderer.SetWRid(wRoleId);
                    roleRenderer.Ctor();

                    roleLogic = domain.SpawnRoleLogic(fieldEntity.transform);
                    roleLogic.Ctor();
                    roleLogic.Inject(roleRenderer);
                    roleLogic.IDComponent.SetEntityId(wRoleId);

                    roleRepo.Add(roleLogic);

                    if (stateMsg.isOwner && roleRepo.Owner == null)
                    {
                        Debug.Log($"生成Owner  wRid:{roleLogic.IDComponent.EntityId})");
                        roleRepo.SetOwner(roleLogic);
                        var fieldCameraComponent = fieldEntity.CameraComponent;
                        fieldCameraComponent.OpenThirdViewCam(roleLogic.roleRenderer);
                    }
                    else
                    {
                        Debug.Log($"人物状态同步帧(roleLogic[{wRoleId}]丢失，重新生成)");
                    }
                }

                if (roleLogic.HealthComponent.IsDead())
                {
                    battleFacades.Domain.RoleDomain.RebornRole(roleLogic);
                    return;
                }

                var animatorComponent = roleLogic.roleRenderer.AnimatorComponent;
                var moveComponent = roleLogic.MoveComponent;
                var weaponComponent = roleLogic.WeaponComponent;

                switch (roleState)
                {
                    case RoleState.Normal:
                        break;
                    case RoleState.Move:
                        if (moveComponent.IsGrouded && weaponComponent.CurrentWeapon == null)
                        {
                            animatorComponent.PlayRunning();
                        }
                        if (moveComponent.IsGrouded && weaponComponent.CurrentWeapon != null)
                        {
                            animatorComponent.PlayRunWithGun();
                        }

                        break;
                    case RoleState.RollForward:
                        if (roleLogic.RoleState != RoleState.RollForward)
                        {
                            animatorComponent.PlayRollForward();
                        }

                        break;
                    case RoleState.Hooking:
                        animatorComponent.PlayHooking();

                        break;
                }

                moveComponent.SetCurPos(pos);
                moveComponent.SetMoveVelocity(moveVelocity);
                moveComponent.SetExtraVelocity(extraVelocity);
                moveComponent.SetGravityVelocity(gravityVelocity);

                if (roleRepo.Owner == null || roleRepo.Owner.IDComponent.EntityId != roleLogic.IDComponent.EntityId)
                {
                    moveComponent.SetEulerAngle(eulerAngle);
                }

                roleLogic.SetRoleState(roleState);
            }
        }

        void Tick_RoleSpawn()
        {
            if (roleSpawnQueue.TryPeek(out var spawn))
            {
                roleSpawnQueue.Dequeue();
                Debug.Log($"生成人物帧 : {spawn.serverFrame}");

                var wRoleId = spawn.wRoleId;
                var repo = battleFacades.Repo;
                var fieldEntity = repo.FiledRepo.Get(1);
                var domain = battleFacades.Domain.RoleDomain;

                var roleRenderer = domain.SpawnRoleRenderer(fieldEntity.Role_Group_Renderer);
                roleRenderer.SetWRid(wRoleId);
                roleRenderer.Ctor();

                var roleLogic = domain.SpawnRoleLogic(fieldEntity.Role_Group_Logic);
                roleLogic.Ctor();
                roleLogic.Inject(roleRenderer);
                roleLogic.IDComponent.SetEntityId(wRoleId);

                var roleRepo = repo.RoleRepo;
                roleRepo.Add(roleLogic);

                var fieldCameraComponent = fieldEntity.CameraComponent;
                if (spawn.isOwner)
                {
                    roleRepo.SetOwner(roleLogic);
                    fieldCameraComponent.OpenThirdViewCam(roleLogic.roleRenderer);
                }

                Debug.Log(spawn.isOwner ? $"生成自身角色 : WRid:{roleLogic.IDComponent.EntityId}" : $"生成其他角色 : WRid:{roleLogic.IDComponent.EntityId}");
            }
        }
        #endregion

        #region [Bullet]
        void Tick_BulletSpawn()
        {
            if (bulletSpawnQueue.TryPeek(out var bulletSpawn))
            {
                bulletSpawnQueue.Dequeue();

                var bulletId = bulletSpawn.bulletId;
                var bulletTypeByte = bulletSpawn.bulletType;
                var bulletType = (BulletType)bulletTypeByte;
                var masterWRid = bulletSpawn.wRid;
                var masterWRole = battleFacades.Repo.RoleRepo.GetByEntityId(masterWRid);
                var shootStartPoint = masterWRole.ShootPointPos;
                Vector3 shootDir = new Vector3(bulletSpawn.shootDirX / 100f, bulletSpawn.shootDirY / 100f, bulletSpawn.shootDirZ / 100f);
                var fieldEntity = battleFacades.Repo.FiledRepo.Get(1);
                var bulletEntity = battleFacades.Domain.BulletDomain.SpawnBullet(fieldEntity.transform, bulletType);

                Debug.Log($"生成子弹帧 {bulletSpawn.serverFrame}: masterWRid:{masterWRid}   起点位置：{shootStartPoint} 飞行方向{shootDir}");

                switch (bulletType)
                {
                    case BulletType.DefaultBullet:
                        break;
                    case BulletType.Grenade:
                        break;
                    case BulletType.Hooker:
                        var hookerEntity = (HookerEntity)bulletEntity;
                        hookerEntity.SetMasterWRid(masterWRid);
                        hookerEntity.SetMasterGrabPoint(masterWRole.transform);

                        break;
                }

                bulletEntity.MoveComponent.SetCurPos(shootStartPoint);
                bulletEntity.MoveComponent.SetForward(shootDir);
                bulletEntity.MoveComponent.ActivateMoveVelocity(shootDir);
                bulletEntity.SetMasterId(masterWRid);
                bulletEntity.IDComponent.SetEntityId(bulletId);
                var bulletRepo = battleFacades.Repo.BulletRepo;
                bulletEntity.gameObject.SetActive(true);
                bulletRepo.Add(bulletEntity);
            }
        }

        void Tick_BulletHitRole()
        {
            while (bulletHitRoleQueue.TryPeek(out var bulletHitRoleMsg))
            {
                bulletHitRoleQueue.Dequeue();

                var bulletRepo = battleFacades.Repo.BulletRepo;
                var roleRepo = battleFacades.Repo.RoleRepo;
                var bullet = bulletRepo.GetByBulletId(bulletHitRoleMsg.bulletId);
                var role = roleRepo.GetByEntityId(bulletHitRoleMsg.entityId);

                role.MoveComponent.HitByBullet(bullet);

                if (role.HealthComponent.IsDead())
                {
                    battleFacades.Domain.RoleDomain.RebornRole(role);
                }

                if (bullet.BulletType == BulletType.DefaultBullet)
                {
                }
                else if (bullet is HookerEntity hookerEntity)
                {
                    // 如果是爪钩则是抓住某物而不是销毁
                    hookerEntity.TryGrabSomthing(role.transform);
                }
            }
        }

        void Tick_BulletHitWall()
        {
            while (bulletHitWallQueue.TryPeek(out var bulletHitWallResMsg))
            {
                bulletHitWallQueue.Dequeue();

                var bulletHitPos = new Vector3(bulletHitWallResMsg.posX / 10000f, bulletHitWallResMsg.posY / 10000f, bulletHitWallResMsg.posZ / 10000f);
                var bulletRepo = battleFacades.Repo.BulletRepo;
                var roleRepo = battleFacades.Repo.RoleRepo;
                var bullet = bulletRepo.GetByBulletId(bulletHitWallResMsg.bulletId);

                if (bullet == null)
                {
                    bulletRepo.TryRemove(bullet);
                    continue;
                }

                if (bullet.BulletType == BulletType.DefaultBullet)
                {
                    bullet.TearDown();
                    bulletRepo.TryRemove(bullet);
                }
                else if (bullet is HookerEntity hookerEntity)
                {
                    // 爪钩:抓住某物而不是销毁
                    hookerEntity.TryGrabPosition(bulletHitPos);
                }
                else if (bullet is GrenadeEntity grenadeEntity)
                {
                    // 手雷:速度清零
                    bullet.MoveComponent.SetMoveVelocity(Vector3.zero);
                }

            }
        }

        void Tick_BulletTearDown()
        {
            while (bulletTearDownQueue.TryDequeue(out var msg))
            {
                var bulletId = msg.bulletId;
                var bulletType = msg.bulletType;
                var bulletRepo = battleFacades.Repo.BulletRepo;
                var bulletEntity = bulletRepo.GetByBulletId(bulletId);

                Vector3 pos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);
                bulletEntity.MoveComponent.SetCurPos(pos);

                if (bulletEntity.BulletType == BulletType.DefaultBullet)
                {
                    bulletEntity.TearDown();
                }

                if (bulletEntity is GrenadeEntity grenadeEntity)
                {
                    grenadeEntity.TearDown();
                }

                if (bulletEntity is HookerEntity hookerEntity)
                {
                    hookerEntity.TearDown();
                }

                bulletRepo.TryRemove(bulletEntity);
            }
        }
        #endregion

        #region [Item]
        void Tick_ItemAssetsSpawn()
        {
            if (itemSpawnQueue.TryPeek(out var itemSpawnMsg))
            {
                itemSpawnQueue.Dequeue();

                var itemTypeArray = itemSpawnMsg.itemTypeArray;
                var subtypeArray = itemSpawnMsg.subtypeArray;
                var entityIdArray = itemSpawnMsg.entityIdArray;
                var fieldEntity = battleFacades.Repo.FiledRepo.CurFieldEntity;
                AssetPointEntity[] assetPointEntities = fieldEntity.transform.GetComponentsInChildren<AssetPointEntity>();

                for (int index = 0; index < assetPointEntities.Length; index++)
                {
                    Transform parent = assetPointEntities[index].transform;
                    ItemType itemType = (ItemType)itemTypeArray[index];
                    ushort entityId = entityIdArray[index];
                    byte subtype = subtypeArray[index];

                    // 生成资源
                    var itemDomain = battleFacades.Domain.ItemDomain;
                    var item = itemDomain.SpawnItem(itemType, subtype);
                    item.transform.SetParent(parent);
                    item.transform.localPosition = Vector3.zero;
                    item.name += entityId;

                    // Entity以及Repo
                    switch (itemType)
                    {
                        case ItemType.Default:
                            break;
                        case ItemType.Weapon:
                            var weaponEntity = item.GetComponent<WeaponEntity>();
                            var weaponRepo = battleFacades.Repo.WeaponRepo;
                            weaponEntity.Ctor();
                            weaponEntity.SetEntityId(entityId);
                            weaponRepo.Add(weaponEntity);
                            break;
                        case ItemType.BulletPack:
                            var bulletPackEntity = item.GetComponent<BulletPackEntity>();
                            var bulletPackRepo = battleFacades.Repo.BulletPackRepo;
                            bulletPackEntity.Ctor();
                            bulletPackEntity.SetEntityId(entityId);
                            bulletPackRepo.Add(bulletPackEntity);
                            break;
                        case ItemType.Pill:
                            break;
                    }
                }
                Debug.Log($"地图资源生成完毕******************************************************");
            }
        }

        void Tick_ItemPick()
        {
            if (itemPickQueue.TryPeek(out var msg))
            {
                itemPickQueue.Dequeue();

                var masterWRID = msg.wRid;
                var itemType = (ItemType)msg.itemType;
                var itemEntityId = msg.entityId;

                var itemDomain = battleFacades.Domain.ItemDomain;
                var repo = battleFacades.Repo;
                var roleRepo = repo.RoleRepo;
                var role = roleRepo.GetByEntityId(msg.wRid);
                if (itemDomain.TryPickUpItem(itemType, itemEntityId, repo, role, role.roleRenderer.handPoint))
                {
                    Debug.Log($"[wRid:{masterWRID}]拾取 {itemType.ToString()}物件[entityId:{itemEntityId}]");
                }

            }
        }
        #endregion

        #endregion

        #region [Server Response]

        #region [ROLE] 
        void OnWRoleSync(BattleRoleStateUpdateMsg msg)
        {
            roleStateQueue.Enqueue(msg);
            // DebugExtensions.LogWithColor($"人物状态同步帧 : {msg.serverFrame}  entityId:{msg.entityId} 角色状态:{msg.roleState.ToString()} 位置 :{new Vector3(msg.x, msg.y, msg.z)} ", "#008000");
        }

        void OnBattleRoleSpawn(FrameBattleRoleSpawnResMsg msg)
        {
            Debug.Log("加入角色生成队列");
            roleSpawnQueue.Enqueue(msg);
        }
        #endregion

        #region [BULLET]
        void OnBulletSpawn(FrameBulletSpawnResMsg msg)
        {
            Debug.Log($"加入子弹生成队列");
            bulletSpawnQueue.Enqueue(msg);
        }

        void OnBulletHitRole(FrameBulletHitRoleResMsg msg)
        {
            Debug.Log("加入子弹击中角色队列");
            bulletHitRoleQueue.Enqueue(msg);
        }

        void OnBulletHitWall(FrameBulletHitWallResMsg msg)
        {
            Debug.Log("加入子弹击中墙壁队列");
            bulletHitWallQueue.Enqueue(msg);
        }

        void OnBulletTearDown(FrameBulletLifeOverResMsg msg)
        {
            Debug.Log($"{msg.bulletType.ToString()} 加入子弹销毁队列");
            bulletTearDownQueue.Enqueue(msg);
        }

        #endregion

        #region [ITEM]
        void OnItemSpawn(FrameItemSpawnResMsg msg)
        {
            Debug.Log($"加入武器生成队列");
            itemSpawnQueue.Enqueue(msg);
        }

        void OnItemPickUp(FrameItemPickResMsg msg)
        {
            Debug.Log($"加入物件拾取队列");
            itemPickQueue.Enqueue(msg);
        }
        #endregion

        // Heartbeat
        void OnHeartbeatRes(BattleHeartbeatResMsg msg)
        {
            Debug.Log("收到心跳");
        }

        #endregion

        #region [Network Event Center]

        async void GameFightStart()
        {
            // // 当前有加载好的场景，则不加载
            // var curFieldEntity = battleFacades.Repo.FiledRepo.CurFieldEntity;
            // if (curFieldEntity != null) return;

            // Load Scene And Spawn Field
            var domain = battleFacades.Domain;
            var fieldEntity = await domain.BattleSpawnDomain.SpawnGameFightScene();
            // Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            fieldEntity.SetFieldId(1);
            var fieldEntityRepo = battleFacades.Repo.FiledRepo;
            var physicsScene = fieldEntity.gameObject.scene.GetPhysicsScene();
            fieldEntityRepo.Add(fieldEntity);
            fieldEntityRepo.SetPhysicsScene(physicsScene);
            // Load UI  
            UIEventCenter.AddToOpen(new OpenEventModel { uiName = "Home_BattleOptPanel" });
            // Send Spawn Role Message
            var rqs = battleFacades.Network.RoleReqAndRes;
            rqs.SendReq_BattleRoleSpawn();

        }

        #endregion

    }

}
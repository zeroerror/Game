using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.WorldBussiness.Interface;
using Game.Client.Bussiness.WorldBussiness.Generic;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {
        WorldFacades worldFacades;
        float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;
        // 服务器下发的生成队列
        Queue<FrameWRoleSpawnResMsg> roleSpawnQueue;
        Queue<FrameBulletSpawnResMsg> bulletSpawnQueue;
        Queue<FrameItemSpawnResMsg> itemSpawnQueue;
        // 服务器下发的物理事件队列
        Queue<FrameBulletHitRoleResMsg> bulletHitRoleQueue;
        Queue<FrameBulletHitWallResMsg> bulletHitWallQueue;
        Queue<FrameBulletTearDownResMsg> bulletTearDownQueue;
        // 服务器下发的人物状态同步队列
        Queue<WRoleStateUpdateMsg> stateQueue;
        // 服务器下发的资源拾取队列
        Queue<FrameItemPickResMsg> itemPickQueue;

        // 服务器下发的武器射击队列
        // 服务器下发的武器换弹队列
        Queue<FrameWeaponReloadReqMsg> weaponReloadQueue;
        // 服务器下发的武器丢弃队列
        Queue<FrameWeaponDropReqMsg> weaponDropQueue;

        public WorldController()
        {
            // Between Bussiness
            NetworkEventCenter.RegistLoginSuccess(EnterWorldChooseScene);

            roleSpawnQueue = new Queue<FrameWRoleSpawnResMsg>();
            bulletSpawnQueue = new Queue<FrameBulletSpawnResMsg>();
            itemSpawnQueue = new Queue<FrameItemSpawnResMsg>();

            bulletHitRoleQueue = new Queue<FrameBulletHitRoleResMsg>();
            bulletHitWallQueue = new Queue<FrameBulletHitWallResMsg>();
            bulletTearDownQueue = new Queue<FrameBulletTearDownResMsg>();

            stateQueue = new Queue<WRoleStateUpdateMsg>();

            itemPickQueue = new Queue<FrameItemPickResMsg>();

            weaponReloadQueue = new Queue<FrameWeaponReloadReqMsg>();
            weaponDropQueue = new Queue<FrameWeaponDropReqMsg>();
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;

            var roleRqs = worldFacades.Network.WorldRoleReqAndRes;
            roleRqs.RegistRes_WorldRoleSpawn(OnWorldRoleSpawn);
            roleRqs.RegistUpdate_WRole(OnWRoleSync);

            var bulletRqs = worldFacades.Network.BulletReqAndRes;
            bulletRqs.RegistRes_BulletSpawn(OnBulletSpawn);
            bulletRqs.RegistRes_BulletHitRole(OnBulletHitRole);
            bulletRqs.RegistRes_BulletHitWall(OnBulletHitWall);
            bulletRqs.RegistRes_BulletTearDown(OnBulletTearDown);

            var weaponRqs = worldFacades.Network.WeaponReqAndRes;
            weaponRqs.RegistRes_WeaponReload(OnWeaponReload);
            weaponRqs.RegistRes_WeaponDrop(OnWeaponDrop);

            var ItemReqAndRes = worldFacades.Network.ItemReqAndRes;
            ItemReqAndRes.RegistRes_ItemSpawn(OnItemSpawn);
            ItemReqAndRes.RegistRes_ItemPickUp(OnItemPickUp);
        }


        public void Tick()
        {
            float deltaTime = UnityEngine.Time.deltaTime;

            // == Server Response Physics 
            Tick_BulletHitWall();
            Tick_BulletHitRole();
            Tick_BulletTearDown();

            // == Server Response
            Tick_RoleSpawn();
            Tick_BulletSpawn();
            Tick_ItemAssetsSpawn();

            Tick_WeaponReload();
            Tick_WeaponDrop();

            Tick_ItemPick();

            Tick_RoleStateSync();

        }

        #region [Input]


        #endregion

        #region [Renderer]

        public void Update_RoleRenderer(float deltaTime)
        {
            var domain = worldFacades.Domain.WorldRoleDomain;
            domain.Update_RoleRenderer(deltaTime);
        }

        public void Update_Camera()
        {
            var curFieldEntity = worldFacades.Repo.FiledEntityRepo.CurFieldEntity;
            if (curFieldEntity == null) return;

            var cameraComponent = curFieldEntity.CameraComponent;
            var currentCam = cameraComponent.CurrentCamera;
            var cameraView = cameraComponent.CurrentCameraView;
            var inputDomain = worldFacades.Domain.WorldInputDomain;
            Vector2 inputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            inputDomain.UpdateCameraByCameraView(worldFacades.Repo.WorldRoleRepo.Owner, cameraView, currentCam, inputAxis);
        }

        #endregion

        #region [Tick Server Resonse]

        #region [Role]
        void Tick_RoleStateSync()
        {
            while (stateQueue.TryPeek(out var stateMsg))
            {
                stateQueue.Dequeue();

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

                var repo = worldFacades.Repo;
                var roleRepo = repo.WorldRoleRepo;
                var fieldRepo = repo.FiledEntityRepo;
                var roleLogic = worldFacades.Repo.WorldRoleRepo.GetByEntityId(stateMsg.wRid);
                if (roleLogic == null)
                {
                    var wRoleId = stateMsg.wRid;
                    var fieldEntity = fieldRepo.Get(1);
                    var domain = worldFacades.Domain.WorldRoleDomain;
                    roleLogic = domain.SpawnWorldRoleLogic(fieldEntity.transform);
                    roleLogic.SetWRid(wRoleId);
                    roleLogic.Ctor();

                    var roleRenderer = domain.SpawnWorldRoleRenderer(fieldEntity.Role_Group_Renderer);
                    roleRenderer.SetWRid(wRoleId);
                    roleRenderer.Ctor();
                    roleLogic.Inject(roleRenderer);

                    roleRepo.Add(roleLogic);
                    if (stateMsg.isOwner && roleRepo.Owner == null)
                    {
                        Debug.Log($"生成Owner  wRid:{roleLogic.WRid})");
                        roleRepo.SetOwner(roleLogic);
                        var fieldCameraComponent = fieldEntity.CameraComponent;
                        fieldCameraComponent.OpenThirdViewCam(roleLogic.roleRenderer);
                    }
                    else
                    {
                        Debug.Log($"人物状态同步帧(roleLogic[{wRoleId}]丢失，重新生成)");
                    }
                }
                // DebugExtensions.LogWithColor($"人物状态同步帧 : {worldClientFrame}  wRid:{stateMsg.wRid} 角色状态:{roleState.ToString()} 位置 :{pos} 移动速度：{moveVelocity} 额外速度：{extraVelocity}  重力速度:{gravityVelocity}  旋转角度：{eulerAngle}","#008000");

                var animatorComponent = roleLogic.roleRenderer.AnimatorComponent;
                var moveComponent = roleLogic.MoveComponent;
                switch (roleState)
                {
                    case RoleState.Normal:
                        animatorComponent.PlayIdle();
                        break;
                    case RoleState.Move:
                        if (moveComponent.IsGrouded) animatorComponent.PlayRun();
                        break;
                    case RoleState.Jump:
                        if (roleLogic.RoleState != RoleState.Jump)
                        {
                            animatorComponent.PlayJump();
                        }
                        moveComponent.TryJump();
                        break;
                    case RoleState.Hooking:
                        animatorComponent.PlayHooking();
                        break;
                }
                moveComponent.SetCurPos(pos);
                if (roleRepo.Owner == null || roleRepo.Owner.WRid != roleLogic.WRid) moveComponent.SetEulerAngle(eulerAngle);
                moveComponent.SetMoveVelocity(moveVelocity);
                moveComponent.SetExtraVelocity(extraVelocity);
                moveComponent.SetGravityVelocity(gravityVelocity);

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
                var repo = worldFacades.Repo;
                var fieldEntity = repo.FiledEntityRepo.Get(1);
                var domain = worldFacades.Domain.WorldRoleDomain;
                var roleLogic = domain.SpawnWorldRoleLogic(fieldEntity.Role_Group_Logic);
                roleLogic.SetWRid(wRoleId);
                roleLogic.Ctor();

                var roleRenderer = domain.SpawnWorldRoleRenderer(fieldEntity.Role_Group_Renderer);
                roleRenderer.SetWRid(wRoleId);
                roleRenderer.Ctor();
                roleLogic.Inject(roleRenderer);

                var roleRepo = repo.WorldRoleRepo;
                roleRepo.Add(roleLogic);

                var fieldCameraComponent = fieldEntity.CameraComponent;
                if (spawn.isOwner)
                {
                    roleRepo.SetOwner(roleLogic);
                    fieldCameraComponent.OpenThirdViewCam(roleLogic.roleRenderer);
                }

                Debug.Log(spawn.isOwner ? $"生成自身角色 : WRid:{roleLogic.WRid}" : $"生成其他角色 : WRid:{roleLogic.WRid}");
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
                var masterWRole = worldFacades.Repo.WorldRoleRepo.GetByEntityId(masterWRid);
                var shootStartPoint = masterWRole.ShootPointPos;
                Vector3 shootDir = new Vector3(bulletSpawn.shootDirX / 100f, bulletSpawn.shootDirY / 100f, bulletSpawn.shootDirZ / 100f);
                Debug.Log($"生成子弹帧 {bulletSpawn.serverFrame}: masterWRid:{masterWRid}   起点位置：{shootStartPoint} 飞行方向{shootDir}");
                var fieldEntity = worldFacades.Repo.FiledEntityRepo.Get(1);
                var bulletEntity = worldFacades.Domain.BulletDomain.SpawnBullet(fieldEntity.transform, bulletType);
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
                bulletEntity.SetEntityId(bulletId);
                var bulletRepo = worldFacades.Repo.BulletRepo;
                bulletEntity.gameObject.SetActive(true);
                bulletRepo.Add(bulletEntity);
            }
        }

        void Tick_BulletHitRole()
        {
            while (bulletHitRoleQueue.TryPeek(out var bulletHitRoleMsg))
            {
                bulletHitRoleQueue.Dequeue();

                var bulletRepo = worldFacades.Repo.BulletRepo;
                var roleRepo = worldFacades.Repo.WorldRoleRepo;
                var bullet = bulletRepo.GetByBulletId(bulletHitRoleMsg.bulletId);
                var role = roleRepo.GetByEntityId(bulletHitRoleMsg.wRid);

                // Client Logic
                role.HealthComponent.HurtByBullet(bullet);
                role.MoveComponent.HitByBullet(bullet);
                if (role.HealthComponent.IsDead)
                {
                    role.TearDown();
                    role.Reborn();
                }

                if (bullet.BulletType == BulletType.DefaultBullet)
                {
                    bullet.TearDown();
                    bulletRepo.TryRemove(bullet);
                }
                else if (bullet is HookerEntity hookerEntity)
                {
                    // 如果是爪钩则是抓住某物而不是销毁
                    hookerEntity.TryGrabSomthing(role.transform);
                    continue;
                }

            }
        }

        void Tick_BulletHitWall()
        {
            while (bulletHitWallQueue.TryPeek(out var bulletHitWallResMsg))
            {
                bulletHitWallQueue.Dequeue();

                var bulletHitPos = new Vector3(bulletHitWallResMsg.posX / 10000f, bulletHitWallResMsg.posY / 10000f, bulletHitWallResMsg.posZ / 10000f);
                var bulletRepo = worldFacades.Repo.BulletRepo;
                var roleRepo = worldFacades.Repo.WorldRoleRepo;
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
                    hookerEntity.TryGrabSomthing(bulletHitPos);
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
                var bulletRepo = worldFacades.Repo.BulletRepo;
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
                var fieldEntity = worldFacades.Repo.FiledEntityRepo.CurFieldEntity;
                AssetPointEntity[] assetPointEntities = fieldEntity.transform.GetComponentsInChildren<AssetPointEntity>();

                for (int index = 0; index < assetPointEntities.Length; index++)
                {
                    Transform parent = assetPointEntities[index].transform;
                    ItemType itemType = (ItemType)itemTypeArray[index];
                    ushort entityId = entityIdArray[index];
                    byte subtype = subtypeArray[index];

                    // 生成资源
                    var itemDomain = worldFacades.Domain.ItemDomain;
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
                            var weaponRepo = worldFacades.Repo.WeaponRepo;
                            weaponEntity.Ctor();
                            weaponEntity.SetEntityId(entityId);
                            weaponRepo.Add(weaponEntity);
                            break;
                        case ItemType.BulletPack:
                            var bulletPackEntity = item.GetComponent<BulletPackEntity>();
                            var bulletPackRepo = worldFacades.Repo.BulletPackRepo;
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

                var itemDomain = worldFacades.Domain.ItemDomain;
                var repo = worldFacades.Repo;
                var roleRepo = repo.WorldRoleRepo;
                var role = roleRepo.GetByEntityId(msg.wRid);
                Debug.Log(role.roleRenderer.handPoint.name);
                itemDomain.TryPickUpItem(itemType, itemEntityId, repo, role, role.roleRenderer.handPoint);
                Debug.Log($"[wRid:{masterWRID}]拾取 {itemType.ToString()}物件[entityId:{itemEntityId}]");

            }
        }
        #endregion

        #region [Weapon]

        void Tick_WeaponReload()
        {
            if (weaponReloadQueue.TryPeek(out var msg))
            {
                weaponReloadQueue.Dequeue();

                var owner = worldFacades.Repo.WorldRoleRepo.Owner;
                owner.TryWeaponReload();
            }
        }


        void Tick_WeaponDrop()
        {
            if (weaponDropQueue.TryPeek(out var msg))
            {
                weaponDropQueue.Dequeue();

                var owner = worldFacades.Repo.WorldRoleRepo.Owner;
                owner.DropWeapon();
            }
        }

        #endregion

        #endregion

        #region [Server Response]

        #region [ROLE] 
        void OnWRoleSync(WRoleStateUpdateMsg msg)
        {
            stateQueue.Enqueue(msg);
        }

        void OnWorldRoleSpawn(FrameWRoleSpawnResMsg msg)
        {
            // Debug.Log("加入角色生成队列");
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

        void OnBulletTearDown(FrameBulletTearDownResMsg msg)
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

        #region [Weapon]

        void OnWeaponReload(FrameWeaponReloadReqMsg msg)
        {
            Debug.Log($"加入武器换弹结束队列");
            weaponReloadQueue.Enqueue(msg);
        }

        void OnWeaponDrop(FrameWeaponDropReqMsg msg)
        {
            Debug.Log($"加入武器丢弃队列");
            weaponDropQueue.Enqueue(msg);
        }

        #endregion

        #endregion

        #region [Network Event Center]

        async void EnterWorldChooseScene()
        {
            // 当前有加载好的场景，则不加载
            var curFieldEntity = worldFacades.Repo.FiledEntityRepo.CurFieldEntity;
            if (curFieldEntity != null) return;

            // Load Scene And Spawn Field
            var domain = worldFacades.Domain;
            var fieldEntity = await domain.WorldSpawnDomain.SpawnCityScene();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            fieldEntity.SetFieldId(1);
            var fieldEntityRepo = worldFacades.Repo.FiledEntityRepo;
            var physicsScene = fieldEntity.gameObject.scene.GetPhysicsScene();
            fieldEntityRepo.Add(fieldEntity);
            fieldEntityRepo.SetPhysicsScene(physicsScene);
            // Send Spawn Role Message
            var rqs = worldFacades.Network.WorldRoleReqAndRes;
            rqs.SendReq_WolrdRoleSpawn();
        }

        #endregion

    }

}
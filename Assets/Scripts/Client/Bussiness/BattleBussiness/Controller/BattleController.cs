using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Protocol.Battle;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.UIBussiness;

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
        Queue<FrameBulletHitFieldResMsg> bulletHitFieldQueue;
        Queue<FrameBulletLifeOverResMsg> bulletTearDownQueue;

        // 服务器下发的人物状态同步队列
        Queue<BattleRoleSyncMsg> roleQueue;

        // 服务器下发的资源拾取队列
        Queue<FrameItemPickResMsg> itemPickQueue;

        bool hasBattleBegin;
        bool hasSpawnBegin;

        public BattleController()
        {

            roleSpawnQueue = new Queue<FrameBattleRoleSpawnResMsg>();
            bulletSpawnQueue = new Queue<FrameBulletSpawnResMsg>();
            itemSpawnQueue = new Queue<FrameItemSpawnResMsg>();

            bulletHitRoleQueue = new Queue<FrameBulletHitRoleResMsg>();
            bulletHitFieldQueue = new Queue<FrameBulletHitFieldResMsg>();
            bulletTearDownQueue = new Queue<FrameBulletLifeOverResMsg>();

            roleQueue = new Queue<BattleRoleSyncMsg>();

            itemPickQueue = new Queue<FrameItemPickResMsg>();

            UIEventCenter.WorldRoomEnter += ((host, port) =>
            {
                battleFacades.Network.BattleReqAndRes.ConnBattleServer(host, port);
            });
            NetworkEventCenter.Regist_BattleSerConnectHandler(() =>
            {
                hasBattleBegin = true;
            });
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;

            var roleRqs = battleFacades.Network.RoleReqAndRes;
            roleRqs.RegistRes_BattleRoleSpawn(OnBattleRoleSpawn);
            roleRqs.RegistUpdate_WRole(OnRoleSync);

            var bulletRqs = battleFacades.Network.BulletReqAndRes;
            bulletRqs.RegistRes_BulletSpawn(OnBulletSpawn);
            bulletRqs.RegistRes_BulletHitRole(OnBulletHitRole);
            bulletRqs.RegistRes_BulletHitField(OnBulletHitField);
            bulletRqs.RegistRes_BulletTearDown(OnBulletTearDown);

            var ItemReqAndRes = battleFacades.Network.ItemReqAndRes;
            ItemReqAndRes.RegistRes_ItemSpawn(OnItemSpawn);
            ItemReqAndRes.RegistRes_ItemPickUp(OnItemPickUp);
        }

        public void Tick()
        {
            if (hasBattleBegin && !hasSpawnBegin)
            {
                hasSpawnBegin = true;
                GameFightSpawn();
            }

            if (battleFacades.Repo.FiledRepo.CurFieldEntity == null)
            {
                return;
            }

            float deltaTime = UnityEngine.Time.deltaTime;

            // == Server Response Physics 
            Tick_BulletHitRole();
            Tick_BulletHitWall();
            Tick_BulletTearDown();

            // == Server Response
            Tick_RoleSpawn();
            Tick_RoleSync();

            Tick_BulletSpawn();

            Tick_ItemAssetsSpawn();
            Tick_ItemPick();

        }

        #region [Input]

        #endregion

        #region [Tick Server Resonse]

        #region [Role]
        void Tick_RoleSync()
        {
            while (roleQueue.TryPeek(out var msg))
            {
                roleQueue.Dequeue();

                RoleState roleState = (RoleState)msg.roleState;
                float x = msg.x / 10000f;
                float y = msg.y / 10000f;
                float z = msg.z / 10000f;
                float eulerX = msg.eulerX / 10000f;
                float eulerY = msg.eulerY / 10000f;
                float eulerZ = msg.eulerZ / 10000f;
                float moveVelocityX = msg.moveVelocityX / 10000f;
                float moveVelocityY = msg.moveVelocityY / 10000f;
                float moveVelocityZ = msg.moveVelocityZ / 10000f;
                float extraVelocityX = msg.extraVelocityX / 10000f;
                float extraVelocityY = msg.extraVelocityY / 10000f;
                float extraVelocityZ = msg.extraVelocityZ / 10000f;
                float gravityVelocity = msg.gravityVelocity / 10000f;

                Vector3 pos = new Vector3(x, y, z);
                Vector3 eulerAngle = new Vector3(eulerX, eulerY, eulerZ);
                Vector3 moveVelocity = new Vector3(moveVelocityX, moveVelocityY, moveVelocityZ);
                Vector3 extraVelocity = new Vector3(extraVelocityX, extraVelocityY, extraVelocityZ);

                var repo = battleFacades.Repo;
                var roleRepo = repo.RoleRepo;
                var fieldRepo = repo.FiledRepo;
                var entityId = msg.entityId;
                var roleLogic = battleFacades.Repo.RoleRepo.Get(entityId);

                if (roleLogic == null)
                {
                    roleLogic = battleFacades.Domain.RoleDomain.SpawnRoleWithRenderer(msg.entityId, msg.isOwner);
                }

                var moveComponent = roleLogic.MoveComponent;
                moveComponent.SetPosition(pos);
                moveComponent.SetMoveVelocity(moveVelocity);
                moveComponent.SetExtraVelocity(extraVelocity);
                moveComponent.SetGravityVelocity(gravityVelocity);

                if (roleRepo.Owner == null || roleRepo.Owner.IDComponent.EntityId != roleLogic.IDComponent.EntityId)
                {
                    //不是Owner
                    moveComponent.SetRotation(eulerAngle);
                }

                var stateComponent = roleLogic.StateComponent;
                if (stateComponent.RoleState == RoleState.Reborn && roleState == RoleState.Normal)
                {
                    battleFacades.Domain.RoleDomain.RoleReborn(roleLogic);
                }

                stateComponent.SetRoleState(roleState);
            }
        }

        void Tick_RoleSpawn()
        {
            if (roleSpawnQueue.TryPeek(out var msg))
            {
                roleSpawnQueue.Dequeue();
                Debug.Log($"生成人物帧 : {msg.serverFrame}");

                var entityId = msg.entityId;
                var repo = battleFacades.Repo;
                var roleRepo = repo.RoleRepo;
                var fieldEntity = repo.FiledRepo.CurFieldEntity;
                var domain = battleFacades.Domain.RoleDomain;
                domain.SpawnRoleWithRenderer(entityId, msg.isOwner);
                Debug.Log(msg.isOwner ? $"生成自身角色   entityId: {entityId}" : $"生成其他角色 : entityId: {entityId}");
            }
        }

        #endregion

        #region [Bullet]
        void Tick_BulletSpawn()
        {
            if (bulletSpawnQueue.TryPeek(out var msg))
            {
                bulletSpawnQueue.Dequeue();

                var bulletId = msg.bulletEntityId;
                var bulletTypeByte = msg.bulletType;
                var bulletType = (BulletType)bulletTypeByte;

                Vector3 startPos = new Vector3(msg.startPosX / 10000f, msg.startPosY / 10000f, msg.startPosZ / 10000f);
                Vector3 fireDir = new Vector3(msg.fireDirX / 100f, 0, msg.fireDirZ / 100f);

                var bulletRepo = battleFacades.Repo.BulletRepo;

                var bulletEntity = battleFacades.Domain.BulletDomain.SpawnBullet(bulletType, msg.bulletEntityId, msg.masterEntityId, startPos, fireDir);

                Debug.Log($"生成子弹帧 {msg.serverFrame}: MasterId:{bulletEntity.MasterEntityId} 起点位置：{startPos}  飞行方向{fireDir}");

            }
        }

        void Tick_BulletHitRole()
        {
            while (bulletHitRoleQueue.TryDequeue(out var msg))
            {
                var role = battleFacades.Repo.RoleRepo.Get(msg.roleEntityId);
                if (role == null)
                {
                    continue;
                }

                var bullet = battleFacades.Repo.BulletRepo.Get(msg.bulletEntityId);
                role.HealthComponent.HurtByDamage(bullet.HitPowerModel.damage);
            }
        }

        void Tick_BulletHitWall()
        {
            while (bulletHitFieldQueue.TryDequeue(out var msg))
            {
                // - 同步子弹位置
                Vector3 pos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);
                var bullet = battleFacades.Repo.BulletRepo.Get(msg.bulletEntityID);
                bullet.MoveComponent.SetPosition(pos);
            }
        }

        void Tick_BulletTearDown()
        {
            while (bulletTearDownQueue.TryDequeue(out var msg))
            {
                var bulletId = msg.bulletId;
                var bulletType = msg.bulletType;
                var bulletRepo = battleFacades.Repo.BulletRepo;
                var bulletEntity = bulletRepo.Get(bulletId);

                Vector3 pos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);
                bulletEntity.MoveComponent.SetPosition(pos);

                if (bulletEntity.BulletType == BulletType.DefaultBullet)
                {
                    bulletEntity.TearDown();
                }

                if (bulletEntity is GrenadeEntity grenadeEntity)
                {
                    var bulletDomain = battleFacades.Domain.BulletDomain;
                    bulletDomain.GrenadeExplode(grenadeEntity, fixedDeltaTime);
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
                var role = roleRepo.Get(msg.wRid);
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
        void OnRoleSync(BattleRoleSyncMsg msg)
        {
            roleQueue.Enqueue(msg);
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

        void OnBulletHitField(FrameBulletHitFieldResMsg msg)
        {
            Debug.Log("加入子弹击中墙壁队列");
            bulletHitFieldQueue.Enqueue(msg);
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

        #endregion

        #region [Network Event Center]

        async void GameFightSpawn()
        {
            Debug.Log($"开始加载战斗场景---------------------------------------------------");

            // Load Scene And Spawn Field
            var domain = battleFacades.Domain;
            var fieldEntity = await domain.SceneDomain.SpawnGameFightScene();
            // Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;

            fieldEntity.SetEntityId(1);

            var fieldEntityRepo = battleFacades.Repo.FiledRepo;
            fieldEntityRepo.Add(fieldEntity);

            var physicsScene = fieldEntity.gameObject.scene.GetPhysicsScene();
            fieldEntityRepo.SetPhysicsScene(physicsScene);

            UIEventCenter.AddToOpen(new OpenEventModel { uiName = "Home_BattleOptPanel" });

            var rqs = battleFacades.Network.RoleReqAndRes;
            rqs.SendReq_BattleRoleSpawn();

            Debug.Log($"加载战斗场景结束---------------------------------------------------");
        }

        #endregion

    }

}
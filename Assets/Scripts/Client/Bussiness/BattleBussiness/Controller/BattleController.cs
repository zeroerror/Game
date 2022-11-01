using System.Collections.Generic;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattleController
    {
        BattleFacades battleFacades;

        // 服务器下发的生成队列
        Queue<FrameBattleRoleSpawnResMsg> roleSpawnQueue;
        Queue<FrameItemSpawnResMsg> itemSpawnQueue;

        // 服务器下发的人物状态同步队列
        Queue<BattleRoleSyncMsg> roleQueue;

        // 服务器下发的资源拾取队列
        Queue<FrameItemPickResMsg> itemPickQueue;

        bool hasBattleBegin;
        bool hasSpawnBegin;

        public BattleController()
        {

            roleSpawnQueue = new Queue<FrameBattleRoleSpawnResMsg>();
            itemSpawnQueue = new Queue<FrameItemSpawnResMsg>();

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

            var ItemReqAndRes = battleFacades.Network.ItemReqAndRes;
            ItemReqAndRes.RegistRes_ItemSpawn(OnItemSpawn);
            ItemReqAndRes.RegistRes_ItemPickUp(OnItemPickUp);
        }

        public void Tick(float fixedDeltaTime)
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

            // == Server Response
            Tick_RoleSpawn();
            Tick_RoleSync();

            Tick_ItemAssetsSpawn();
            Tick_ItemPick();

        }

        #region [Role Tick]
        void Tick_RoleSync()
        {
            while (roleQueue.TryPeek(out var msg))
            {
                roleQueue.Dequeue();

                RoleState roleState = (RoleState)msg.roleState;
                float x = msg.posX / 10000f;
                float y = msg.posY / 10000f;
                float z = msg.posZ / 10000f;
                float eulerX = msg.eulerX / 10000f;
                float eulerY = msg.eulerY / 10000f;
                float eulerZ = msg.eulerZ / 10000f;
                float velocityX = msg.velocityX / 10000f;
                float velocityY = msg.velocityY / 10000f;
                float velocityZ = msg.velocityZ / 10000f;

                Vector3 pos = new Vector3(x, y, z);
                Vector3 eulerAngle = new Vector3(eulerX, eulerY, eulerZ);
                Vector3 velocity = new Vector3(velocityX, velocityY, velocityZ);

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
                moveComponent.SetVelocity(velocity);

                if (roleRepo.Owner == null || roleRepo.Owner.IDComponent.EntityID != roleLogic.IDComponent.EntityID)
                {
                    //不是Owner
                    moveComponent.SetRotation(eulerAngle);
                }

                var stateComponent = roleLogic.StateComponent;
                if (stateComponent.RoleState == RoleState.Reborning && roleState == RoleState.Normal)
                {
                    battleFacades.Domain.RoleDomain.Reborn(roleLogic);
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

        #region [Item Tick]
        void Tick_ItemAssetsSpawn()
        {
            if (itemSpawnQueue.TryPeek(out var itemSpawnMsg))
            {
                itemSpawnQueue.Dequeue();

                var entityTypeArray = itemSpawnMsg.entityTypeArray;
                var subtypeArray = itemSpawnMsg.subtypeArray;
                var entityIDArray = itemSpawnMsg.entityIDArray;
                var fieldEntity = battleFacades.Repo.FiledRepo.CurFieldEntity;
                AssetPointEntity[] assetPointEntities = fieldEntity.transform.GetComponentsInChildren<AssetPointEntity>();

                for (int index = 0; index < assetPointEntities.Length; index++)
                {
                    Transform parent = assetPointEntities[index].transform;
                    EntityType entityType = (EntityType)entityTypeArray[index];
                    int entityID = entityIDArray[index];
                    byte subtype = subtypeArray[index];

                    // 生成资源
                    var itemDomain = battleFacades.Domain.ItemDomain;
                    var itemGo = itemDomain.SpawnItem(entityType, subtype, entityID, parent);
                    itemGo.transform.SetParent(parent);
                    itemGo.transform.localPosition = Vector3.zero;
                    itemGo.name += entityID;
                }
                Debug.Log($"地图资源生成完毕******************************************************");
            }
        }

        void Tick_ItemPick()
        {
            if (itemPickQueue.TryPeek(out var msg))
            {
                itemPickQueue.Dequeue();

                var masterEntityID = msg.masterEntityID;
                var entityType = (EntityType)msg.itemType;
                var itemEntityId = msg.itemEntityID;

                var repo = battleFacades.Repo;
                var roleRepo = repo.RoleRepo;
                var role = roleRepo.Get(masterEntityID);

                var itemDomain = battleFacades.Domain.ItemDomain;
                if (itemDomain.TryPickUpItem(entityType, itemEntityId, masterEntityID, role.roleRenderer.handPoint))
                {
                    Debug.Log($"[MasterEntityID:{masterEntityID}] 拾取 {entityType.ToString()} [EntityID:{itemEntityId}]");
                }

            }
        }
        #endregion

        #region [Role Ser Res] 
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

        #region [Item Ser Res]
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
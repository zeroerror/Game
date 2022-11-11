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

        public BattleController()
        {
            battleRoleSyncQueue = new Queue<BattleRoleSyncMsg>();
            roleSpawnQueue = new Queue<BattleRoleSpawnResMsg>();

            itemSpawnQueue = new Queue<BattleAssetPointItemsSpawnResMsg>();
            itemPickQueue = new Queue<BattleItemPickResMsg>();

            airdropSpawnQueue = new Queue<BattleAirdropSpawnResMsg>();
            airdropTearDownQueue = new Queue<BattleAirdropTearDownResMsg>();

        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;

            // --- Network Events3
            // - Battle
            var battleRqs = battleFacades.Network.BattleReqAndRes;
            NetworkEventCenter.Regist_BattleSerConnectAction(() =>
            {
                battleRqs.SendReq_BattleGameStateAndStage();

                var rqs = battleFacades.Network.RoleReqAndRes;
                rqs.SendReq_RoleSpawn(ControlType.Owner);
            });
            battleRqs.RegistRes_BattleGameStateAndStage(OnRes_BattleStateAndStageMsg);
            battleRqs.RegistRes_BattleAirdropSpawn(OnRes_BattleAirdropSpawn);
            battleRqs.RegistRes_BattleAirdropTearDown(OnRes_BattleAirdropTearDown);

            // - Role
            var roleRqs = battleFacades.Network.RoleReqAndRes;
            roleRqs.RegistRes_BattleRoleSpawn(OnRoleSpawn);
            roleRqs.RegistUpdate_WRole(OnRoleSync);

            // - Item
            var ItemReqAndRes = battleFacades.Network.ItemReqAndRes;
            ItemReqAndRes.RegistRes_ItemSpawn(OnItemSpawn);
            ItemReqAndRes.RegistRes_ItemPickUp(OnItemPickUp);

            // --- Local Events
            UIEventCenter.World_EnterRoom += ((host, port) =>
            {
                var battleRqs = battleFacades.Network.BattleReqAndRes;
                battleRqs.ConnBattleServer(host, port);
            });

            var logicEventCenter = battleFacades.LogicEventCenter;
            logicEventCenter.Regist_BattleStateAndStageChangeAction(LogicEvent_BattleStateAndStageChange);
        }

        public void Tick(float fixedDeltaTime)
        {
            // - Game State Apply
            var gameStateDomain = battleFacades.Domain.BattleStateDomain;
            gameStateDomain.ApplyGameState();

            var gameEntity = battleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var state = fsm.BattleState;
            if (!state.CanBattleLoop())
            {
                return;
            }

            // == Server Response
            Tick_RoleSpawn();
            Tick_RoleSync();

            Tick_ItemAssetsSpawn();
            Tick_ItemPick();

            Tick_AirdropSpawn();
            Tick_AirdropTearDown();
        }

        #region [Battle Role] 

        Queue<BattleRoleSyncMsg> battleRoleSyncQueue;
        Queue<BattleRoleSpawnResMsg> roleSpawnQueue;

        void Tick_RoleSync()
        {
            while (battleRoleSyncQueue.TryPeek(out var msg))
            {
                battleRoleSyncQueue.Dequeue();

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
                var roleRepo = repo.RoleLogicRepo;
                var fieldRepo = repo.FieldRepo;
                var entityID = msg.entityId;

                if (!battleFacades.Repo.RoleLogicRepo.TryGet(entityID, out var roleLogic))
                {
                    var roleDoamin = battleFacades.Domain.RoleLogicDomain;
                    roleLogic = roleDoamin.SpawnRoleWithRenderer(msg.entityId, ControlType.Other);
                }

                var moveComponent = roleLogic.LocomotionComponent;
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
                    battleFacades.Domain.RoleLogicDomain.Reborn(roleLogic);
                }

                stateComponent.SetRoleState(roleState);
            }
        }

        void Tick_RoleSpawn()
        {
            while (roleSpawnQueue.TryPeek(out var msg))
            {
                roleSpawnQueue.Dequeue();

                var entityId = msg.entityId;
                var repo = battleFacades.Repo;
                var roleRepo = repo.RoleLogicRepo;
                var fieldEntity = repo.FieldRepo.CurFieldEntity;
                var roleDomain = battleFacades.Domain.RoleLogicDomain;
                var controlType = (ControlType)msg.controlType;
                roleDomain.SpawnRoleWithRenderer(entityId, controlType);
                Debug.Log($"生成自身角色  ControlType {controlType}");
            }
        }

        void OnRoleSync(BattleRoleSyncMsg msg)
        {
            battleRoleSyncQueue.Enqueue(msg);
            // DebugExtensions.LogWithColor($"人物状态同步帧 : {msg.serverFrame}  entityId:{msg.entityId} 角色状态:{msg.roleState.ToString()} 位置 :{new Vector3(msg.x, msg.y, msg.z)} ", "#008000");
        }

        void OnRoleSpawn(BattleRoleSpawnResMsg msg)
        {
            Debug.Log("收到角色生成消息");
            roleSpawnQueue.Enqueue(msg);
        }
        #endregion

        #region [Item]

        Queue<BattleItemPickResMsg> itemPickQueue;
        Queue<BattleAssetPointItemsSpawnResMsg> itemSpawnQueue;

        void Tick_ItemAssetsSpawn()
        {
            while (itemSpawnQueue.TryPeek(out var msg))
            {
                itemSpawnQueue.Dequeue();

                var entityTypeArray = msg.entityTypeArray;
                var subtypeArray = msg.subtypeArray;
                var entityIDArray = msg.entityIDArray;
                var fieldEntity = battleFacades.Repo.FieldRepo.CurFieldEntity;
                AssetPointEntity[] assetPointEntities = fieldEntity.transform.GetComponentsInChildren<AssetPointEntity>();

                for (int index = 0; index < assetPointEntities.Length; index++)
                {
                    Transform parent = assetPointEntities[index].transform;
                    EntityType entityType = (EntityType)entityTypeArray[index];
                    int entityID = entityIDArray[index];
                    byte subtype = subtypeArray[index];

                    // 生成资源
                    var commonDomain = battleFacades.Domain.CommonDomain;
                    var entityObj = commonDomain.SpawnEntity_Logic(entityType, subtype, entityID, parent.position);
                    var entityGo = commonDomain.UnpackEntityObjToGO(entityObj, entityType);
                    entityGo.transform.SetParent(parent);
                    entityGo.transform.localPosition = Vector3.zero;
                    entityGo.name += entityID;
                }
                Debug.Log($"地图资源生成完毕******************************************************");
            }
        }

        void Tick_ItemPick()
        {
            if (itemPickQueue.TryPeek(out var msg))
            {
                itemPickQueue.Dequeue();

                var masterEntityID = msg.roleID;
                var entityType = (EntityType)msg.entityType;
                var itemEntityId = msg.itemID;

                var repo = battleFacades.Repo;
                var roleRepo = repo.RoleLogicRepo;
                var role = roleRepo.Get(masterEntityID);

                var itemDomain = battleFacades.Domain.ItemDomain;
                if (itemDomain.TryPickUpItem(masterEntityID, entityType, itemEntityId, role.roleRenderer.handPoint))
                {
                    Debug.Log($"[MasterEntityID:{masterEntityID}] 拾取 {entityType.ToString()} [EntityID:{itemEntityId}]");
                }

            }
        }

        void OnItemSpawn(BattleAssetPointItemsSpawnResMsg msg)
        {
            itemSpawnQueue.Enqueue(msg);
        }

        void OnItemPickUp(BattleItemPickResMsg msg)
        {
            itemPickQueue.Enqueue(msg);
        }
        #endregion

        #region [Battle State]

        void LogicEvent_BattleStateAndStageChange()
        {
            // - Sync With Server
            var battleRqs = battleFacades.Network.BattleReqAndRes;
            battleRqs.SendReq_BattleGameStateAndStage();
        }

        void OnRes_BattleStateAndStageMsg(BattleStateAndStageResMsg msg)
        {
            var gameEntity = battleFacades.GameEntity;
            var stage = gameEntity.Stage;
            var fsm = gameEntity.FSMComponent;
            var state = fsm.BattleState;

            BattleStage serStage = (BattleStage)msg.stage;
            BattleState serState = (BattleState)msg.state;
            int curMaintainFrame = msg.curMaintainFrame;

            var curSerStageLevel = serStage.GetCurLevelStage();
            bool isClientFieldSpawned = stage.CompareStage(serStage, curSerStageLevel) == 0;
            if (!isClientFieldSpawned)
            {
                if (state != BattleState.SpawningField)
                {
                    fsm.EnterGameState_BattleSpawningField(BattleStage.Level1);
                }
                return;
            }

            if (serState == BattleState.SpawningField)
            {
                fsm.EnterGameState_BattleSpawningField(curSerStageLevel);
                return;
            }

            if (serState == BattleState.Preparing)
            {
                fsm.EnterGameState_BattlePreparing(curMaintainFrame);
                UIEventCenter.AddToOpen(new OpenEventModel { uiName = "Home_BattleOptPanel" });
                UIEventCenter.AddToOpen(new OpenEventModel { uiName = "Home_BattleInfoPanel" });
                return;
            }

            if (serState == BattleState.Fighting)
            {
                fsm.EnterGameState_BattleFighting(curMaintainFrame);
                return;
            }

            if (serState == BattleState.Settlement)
            {
                fsm.EnterGameState_BattleSettlement(curMaintainFrame);
                return;
            }

            Debug.LogWarning("None Taken");
            return;
        }

        #endregion

        #region [Battle Airdrop]

        Queue<BattleAirdropSpawnResMsg> airdropSpawnQueue;
        Queue<BattleAirdropTearDownResMsg> airdropTearDownQueue;

        void Tick_AirdropSpawn()
        {
            while (airdropSpawnQueue.TryDequeue(out var msg))
            {
                EntityType airdropEntityType = (EntityType)msg.airdropEntityType;
                byte subType = msg.subType;
                int entityID = msg.entityID;
                Vector3 spawnPos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);
                BattleStage curLvStage = (BattleStage)msg.curLvStage;

                var domain = battleFacades.Domain;
                var airdropLogicDomain = domain.AirdropLogicDomain;
                airdropLogicDomain.SpawnLogic(curLvStage, entityID, spawnPos);
                var airdropRendererDomain = domain.AirdropRendererDomain;
                airdropRendererDomain.SpawnRenderer(entityID, curLvStage, spawnPos);
            }
        }

        void OnRes_BattleAirdropSpawn(BattleAirdropSpawnResMsg msg)
        {
            airdropSpawnQueue.Enqueue(msg);
        }

        void Tick_AirdropTearDown()
        {
            while (airdropTearDownQueue.TryDequeue(out var msg))
            {
                int airdropID = msg.airdropID;
                EntityType spawnEntityType = (EntityType)msg.spawnEntityType;
                byte spawnSubType = msg.spawnSubType;
                int spawnEntityID = msg.spawnEntityID;
                Vector3 spawnPos = new Vector3(msg.spawnPosX / 10000f, msg.spawnPosY / 10000f, msg.spawnPosZ / 10000f);

                var domain = battleFacades.Domain;
                var commonDomain = domain.CommonDomain;
                commonDomain.SpawnEntity_Logic(spawnEntityType, spawnSubType, spawnEntityID, spawnPos);
                commonDomain.SpawnEntity_Renderer(spawnEntityType, spawnSubType, spawnEntityID, spawnPos);

                var airdropLogicDomain = domain.AirdropLogicDomain;
                airdropLogicDomain.TearDownLogic(airdropID);
                var airdropRendererDomain = domain.AirdropRendererDomain;
                airdropRendererDomain.TearDownRenderer(airdropID);
            }
        }

        void OnRes_BattleAirdropTearDown(BattleAirdropTearDownResMsg msg)
        {
            airdropTearDownQueue.Enqueue(msg);
        }

        #endregion

    }

}
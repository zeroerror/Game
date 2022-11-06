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

        public BattleController()
        {
            roleQueue = new Queue<BattleRoleSyncMsg>();
            roleSpawnQueue = new Queue<FrameBattleRoleSpawnResMsg>();

            itemSpawnQueue = new Queue<FrameItemSpawnResMsg>();
            itemPickQueue = new Queue<FrameItemPickResMsg>();
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;

            var battleRqs = battleFacades.Network.BattleReqAndRes;
            battleRqs.RegistRes_BattleGameStateAndStage(OnBattleStateAndStageResMsg);

            var roleRqs = battleFacades.Network.RoleReqAndRes;
            roleRqs.RegistRes_BattleRoleSpawn(OnBattleRoleSpawn);
            roleRqs.RegistUpdate_WRole(OnRoleSync);

            var ItemReqAndRes = battleFacades.Network.ItemReqAndRes;
            ItemReqAndRes.RegistRes_ItemSpawn(OnItemSpawn);
            ItemReqAndRes.RegistRes_ItemPickUp(OnItemPickUp);

            var BattleStateDomain = battleFacades.Domain.BattleStateDomain;
            BattleStateDomain.RegistStateAndStageChangeHandler(OnBattleStateAndStageChange);

            UIEventCenter.WorldRoomEnter += ((host, port) =>
            {
                var battleRqs = battleFacades.Network.BattleReqAndRes;
                battleRqs.ConnBattleServer(host, port);
            });
            NetworkEventCenter.Regist_BattleSerConnectHandler(() =>
            {
                var battleRqs = battleFacades.Network.BattleReqAndRes;
                battleRqs.SendReq_BattleGameStateAndStage();

                var rqs = battleFacades.Network.RoleReqAndRes;
                rqs.SendReq_RoleSpawn(ControlType.Owner);
            });
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
                var roleRepo = repo.RoleLogicRepo;
                var fieldRepo = repo.FiledRepo;
                var entityID = msg.entityId;

                if (!battleFacades.Repo.RoleLogicRepo.TryGet(entityID, out var roleLogic))
                {
                    var roleDoamin = battleFacades.Domain.RoleDomain;
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
                    battleFacades.Domain.RoleDomain.Reborn(roleLogic);
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
                var fieldEntity = repo.FiledRepo.CurFieldEntity;
                var domain = battleFacades.Domain.RoleDomain;
                var controlType = (ControlType)msg.controlType;
                domain.SpawnRoleWithRenderer(entityId, controlType);
                Debug.Log($"生成自身角色  ControlType {controlType}");
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

        void OnBattleStateAndStageResMsg(BattleStateAndStageResMsg msg)
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

        void OnBattleStateAndStageChange()
        {
            // - Sync With Server
            var battleRqs = battleFacades.Network.BattleReqAndRes;
            battleRqs.SendReq_BattleGameStateAndStage();
        }

    }

}
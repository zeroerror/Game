using System.Collections.Generic;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Server.Bussiness.EventCenter;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleController
    {
        ServerBattleFacades serverFacades;

        // NetWork Info
        public int ServerFrame => serverFacades.Network.ServeFrame;
        List<int> ConnIDList => serverFacades.Network.connIdList;

        // ====== 角色 ======
        // - 所有生成帧
        Dictionary<long, BattleRoleSpawnReqMsg> roleSpawnMsgDic;
        // -所有操作帧
        Dictionary<long, BattleRoleMoveReqMsg> roleMoveMsgDic;
        Dictionary<long, BattleRoleRotateReqMsg> roleRotateMsgDic;
        // - 所有跳跃帧
        Dictionary<long, BattleRoleRollReqMsg> rollOptMsgDic;

        // ====== 子弹 ======
        // - 所有拾取物件帧
        Dictionary<long, BattleItemPickReqMsg> itemPickUpMsgDic;

        public BattleController()
        {
            roleSpawnMsgDic = new Dictionary<long, BattleRoleSpawnReqMsg>();
            roleMoveMsgDic = new Dictionary<long, BattleRoleMoveReqMsg>();
            roleRotateMsgDic = new Dictionary<long, BattleRoleRotateReqMsg>();
            rollOptMsgDic = new Dictionary<long, BattleRoleRollReqMsg>();

            itemPickUpMsgDic = new Dictionary<long, BattleItemPickReqMsg>();
        }

        public void Inject(ServerBattleFacades v)
        {
            serverFacades = v;

            // - Network
            var battleRqs = serverFacades.Network.BattleReqAndRes;
            battleRqs.RegistReq_BattleGameStateAndStage(OnBattleStateAndStageReqMsg);

            var roleRqs = v.Network.RoleReqAndRes;
            roleRqs.RegistReq_RoleMove(OnRoleMoveReqMsg);
            roleRqs.RegistReq_RoleRotate(OnRoleRotateReqMsg);
            roleRqs.RegistReq_Jump(OnRoleRollReqMsg);
            roleRqs.RegistReq_BattleRoleSpawn(OnRoleSpawnReqMsg);

            var itemRqs = v.Network.ItemReqAndRes;
            itemRqs.RegistReq_ItemPickUp(OnItemPickUpReqMsg);

            // Domain Handler
            var battleFacades = serverFacades.BattleFacades;
            var logicEventCenter = battleFacades.LogicEventCenter;
            logicEventCenter.Regist_BattleStateAndStageChangeAction(LogicEvent_BattleStateAndStageChange);
            logicEventCenter.Regist_BattleAirDropAction(LogicEvent_BattleAirdrop);
        }

        public void Tick(float fixedDeltaTime)
        {
            // - Role
            Tick_RoleSpawn();
            Tick_RoleRollOpt();
            Tick_RoleMoveRotateOpt();
            ApplyAllRoleState();

            // - Item
            Tick_ItemPickUp();

            // - Broadcast
            BroadcastAllRoleState();
        }

        #region [Role]

        void BroadcastAllRoleState()
        {
            var roleRqs = serverFacades.Network.RoleReqAndRes;
            var roleRepo = serverFacades.BattleFacades.Repo.RoleLogicRepo;
            ConnIDList.ForEach((connID) =>
            {
                roleRepo.Foreach((role) =>
                {
                    roleRqs.SendUpdate_RoleState(connID, role);
                });
            });
        }

        void Tick_RoleSpawn()
        {
            ConnIDList.ForEach((connID) =>
            {
                long key = GetCurFrameKey(connID);

                if (roleSpawnMsgDic.TryGetValue(key, out var msg))
                {
                    if (msg == null)
                    {
                        return;
                    }

                    roleSpawnMsgDic[key] = null;

                    var battleFacades = serverFacades.BattleFacades;
                    var repo = battleFacades.Repo;
                    var roleRepo = repo.RoleLogicRepo;
                    if (!roleRepo.TryGetByConnID(connID, out var role))
                    {
                        var idService = battleFacades.IDService;
                        var entityID = idService.GetAutoIDByEntityType(EntityType.BattleRole);
                        role = battleFacades.Domain.RoleLogicDomain.SpawnLogic(entityID);
                        role.SetConnID(connID);
                        Debug.Log($"服务器逻辑[生成角色] ServerFrame:{ServerFrame} EntityID:{entityID} ControlType {((ControlType)msg.controlType).ToString()}");
                    }

                    var serNetwork = serverFacades.Network;
                    var roleRqs = serNetwork.RoleReqAndRes;
                    roleRqs.SendRes_BattleRoleSpawn(connID, role.IDComponent.EntityID, msg.controlType);
                }
            });
        }

        void OnRoleSpawnReqMsg(int connID, BattleRoleSpawnReqMsg msg)
        {
            lock (roleSpawnMsgDic)
            {
                long key = GetCurFrameKey(connID);
                if (!roleSpawnMsgDic.TryGetValue(key, out var _))
                {
                    roleSpawnMsgDic[key] = msg;
                }
            }
        }
        
        void Tick_RoleMoveRotateOpt()
        {
            ConnIDList.ForEach((connId) =>
            {
                long key = GetCurFrameKey(connId);

                var roleRepo = serverFacades.BattleFacades.Repo.RoleLogicRepo;
                var rqs = serverFacades.Network.RoleReqAndRes;

                if (roleMoveMsgDic.TryGetValue(key, out var msg))
                {

                    var realMsg = msg.msg;

                    var rid = (byte)(realMsg >> 48);
                    var role = roleRepo.Get(rid);

                    // ------------移动 -> Input
                    Vector3 dir = new Vector3((short)(ushort)(realMsg >> 32) / 100f, (short)(ushort)(realMsg >> 16) / 100f, (short)(ushort)realMsg / 100f);
                    role.InputComponent.SetMoveDir(dir);
                }

                if (roleRotateMsgDic.TryGetValue(key, out var rotateMsg))
                {
                    // ------------转向 -> Input（基于客户端鉴权的同步） 
                    var realMsg = rotateMsg.msg;
                    var rid = (byte)(realMsg >> 48);
                    var role = roleRepo.Get(rid);
                    Vector3 eulerAngle = new Vector3((short)(realMsg >> 32), (short)(realMsg >> 16), (short)realMsg);
                    role.InputComponent.SetFaceDir(eulerAngle);
                }

            });

        }

        void OnRoleMoveReqMsg(int connId, BattleRoleMoveReqMsg msg)
        {
            lock (roleMoveMsgDic)
            {
                long key = GetCurFrameKey(connId);

                if (!roleMoveMsgDic.TryGetValue(key, out var opt))
                {
                    roleMoveMsgDic[key] = msg;
                }

            }
        }
       
        void OnRoleRotateReqMsg(int connId, BattleRoleRotateReqMsg msg)
        {
            lock (roleRotateMsgDic)
            {
                long key = GetCurFrameKey(connId);

                if (!roleRotateMsgDic.TryGetValue(key, out var opt))
                {
                    roleRotateMsgDic[key] = msg;
                }
            }
        }

        void Tick_RoleRollOpt()
        {
            ConnIDList.ForEach((connId) =>
            {
                long key = GetCurFrameKey(connId);
                if (rollOptMsgDic.TryGetValue(key, out var msg))
                {
                    if (msg == null)
                    {
                        return;
                    }

                    rollOptMsgDic[key] = null;

                    var wRid = msg.entityId;
                    var roleRepo = serverFacades.BattleFacades.Repo.RoleLogicRepo;
                    var role = roleRepo.Get(wRid);
                    var rqs = serverFacades.Network.RoleReqAndRes;
                    Vector3 dir = new Vector3(msg.dirX / 10000f, msg.dirY / 10000f, msg.dirZ / 10000f);
                    role.InputComponent.SetRollDir(dir);
                }
            });
        }

        void OnRoleRollReqMsg(int connId, BattleRoleRollReqMsg msg)
        {
            lock (rollOptMsgDic)
            {
                long key = GetCurFrameKey(connId);
                if (!rollOptMsgDic.TryGetValue(key, out var opt))
                {
                    rollOptMsgDic[key] = msg;
                }

            }
        }

        void ApplyAllRoleState()
        {
            var roleStateDomain = serverFacades.BattleFacades.Domain.RoleStateDomain;
            roleStateDomain.ApplyAllRoleState();
        }

        #endregion

        #region [Item]

        void Tick_ItemPickUp()
        {

            ConnIDList.ForEach((connId) =>
            {
                long key = GetCurFrameKey(connId);

                if (itemPickUpMsgDic.TryGetValue(key, out var msg))
                {
                    if (msg == null)
                    {
                        return;
                    }

                    itemPickUpMsgDic[key] = null;

                    // TODO:Add judgement like 'Can He Pick It Up?'
                    EntityType entityType = (EntityType)msg.entityType;
                    var itemDomain = serverFacades.BattleFacades.Domain.ItemDomain;
                    if (itemDomain.TryPickUpItem(msg.roleID, entityType, msg.itemID))
                    {
                        var rqs = serverFacades.Network.ItemReqAndRes;
                        ConnIDList.ForEach((connId) =>
                        {
                            rqs.SendRes_ItemPickUp(connId, ServerFrame, msg.roleID, entityType, msg.itemID);
                        });
                    }
                    else
                    {
                        Debug.Log($"{entityType.ToString()}物品拾取失败");
                    }
                }
            });

        }

        void OnItemPickUpReqMsg(int connId, BattleItemPickReqMsg msg)
        {
            lock (itemPickUpMsgDic)
            {
                long key = GetCurFrameKey(connId);

                if (!itemPickUpMsgDic.TryGetValue(key, out var _))
                {
                    itemPickUpMsgDic[key] = msg;
                }
            }
        }

        #endregion

        void LogicEvent_BattleAirdrop()
        {
            var battleFacades = serverFacades.BattleFacades;

            var curField = battleFacades.Repo.FieldRepo.CurFieldEntity;
            var spawnPos = curField.UseRandomAirdropPos();

            var idService = battleFacades.IDService;
            var entityID = idService.GetAutoIDByEntityType(EntityType.Aridrop);

            var battleStage = battleFacades.GameEntity.Stage;
            var curLvStage = battleStage.GetCurLevelStage();

            var airdropDomain = battleFacades.Domain.AirdropLogicDomain;
            var airdrop = airdropDomain.SpawnLogic(curLvStage, entityID, spawnPos);

            var battleRqs = serverFacades.Network.BattleReqAndRes;
            ConnIDList.ForEach((connID) =>
            {
                battleRqs.SendRes_BattleAirdropSpawn(connID, airdrop.SpawnEntityType, airdrop.SpawnSubType, airdrop.IDComponent.EntityID, spawnPos, curLvStage);
            });
        }

        void LogicEvent_BattleStateAndStageChange()
        {
            var gameEntity = serverFacades.BattleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var state = fsm.BattleState;
            var stage = gameEntity.Stage;
            var battleStateDomain = serverFacades.BattleFacades.Domain.BattleStateDomain;
            int curMaintainFrame = battleStateDomain.GetCurMainTainFrame();

            ConnIDList.ForEach((connID) =>
            {
                var battleRqs = serverFacades.Network.BattleReqAndRes;
                battleRqs.SendRes_BattleGameStateAndStage(connID, state, stage, curMaintainFrame);
            });

            Debug.Log($"OnGameStageChange state {state.ToString()} stage {stage}");

            if (state == BattleState.Preparing)
            {
                var battleFacades = serverFacades.BattleFacades;
                var repo = battleFacades.Repo;
                var domain = battleFacades.Domain;

                // --- 生成
                var fieldDomain = domain.FieldDomain;
                var fieldRepo = repo.FieldRepo;
                var curField = repo.FieldRepo.CurFieldEntity;
                fieldDomain.RandomSpawnAllItemToField(curField, out var entityTypeList, out var subTypeList, out var entityIDList);

                ConnIDList.ForEach((connID) =>
                {
                    var itemRqs = serverFacades.Network.ItemReqAndRes;
                    itemRqs.SendRes_BattleAssetPointItemsSpawn(connID, entityTypeList, subTypeList, entityIDList);
                });
            }

        }

        void OnBattleStateAndStageReqMsg(int connID, BattleStateAndStageReqMsg msg)
        {
            var gameEntity = serverFacades.BattleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var state = fsm.BattleState;
            var stage = gameEntity.Stage;
            var battleStateDomain = serverFacades.BattleFacades.Domain.BattleStateDomain;
            int curMaintainFrame = battleStateDomain.GetCurMainTainFrame();

            var battleRqs = serverFacades.Network.BattleReqAndRes;
            battleRqs.SendRes_BattleGameStateAndStage(connID, state, stage, curMaintainFrame);
        }

        long GetCurFrameKey(int connId)
        {
            long key = (long)ServerFrame << 32;
            key |= (long)connId;
            return key;
        }

    }


}
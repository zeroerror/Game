using System.Collections.Generic;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Client.Bussiness;
using Game.Client.Bussiness.BattleBussiness;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Server.Bussiness.EventCenter;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleController
    {
        BattleServerFacades serverFacades;

        // Scene Spawn Trigger
        bool hasBattleBegin;
        bool hasSpawnBegin;
        bool hasSpawnFinished;

        // NetWork Info
        public int ServerFrame => serverFacades.Network.ServeFrame;
        List<int> ConnIDList => serverFacades.Network.connIdList;

        // ====== 角色 ======
        // - 所有生成帧
        Dictionary<long, FrameBattleRoleSpawnReqMsg> roleSpawnMsgDic;
        // -所有操作帧
        Dictionary<long, FrameRoleMoveReqMsg> roleMoveMsgDic;
        Dictionary<long, FrameRoleRotateReqMsg> roleRotateMsgDic;
        // - 所有跳跃帧
        Dictionary<long, FrameRollReqMsg> rollOptMsgDic;

        // ====== 子弹 ======
        // - 所有拾取物件帧
        Dictionary<long, FrameItemPickReqMsg> itemPickUpMsgDic;

        public BattleController()
        {
            ServerNetworkEventCenter.battleSerConnect += ((connId) =>
            {
                ConnIDList.Add(connId); //添加至连接名单
                hasBattleBegin = true;
            });

            roleSpawnMsgDic = new Dictionary<long, FrameBattleRoleSpawnReqMsg>();
            roleMoveMsgDic = new Dictionary<long, FrameRoleMoveReqMsg>();
            roleRotateMsgDic = new Dictionary<long, FrameRoleRotateReqMsg>();
            rollOptMsgDic = new Dictionary<long, FrameRollReqMsg>();

            itemPickUpMsgDic = new Dictionary<long, FrameItemPickReqMsg>();
        }

        public void Inject(BattleServerFacades battleFacades)
        {
            this.serverFacades = battleFacades;

            var roleRqs = battleFacades.Network.RoleReqAndRes;
            roleRqs.RegistReq_RoleMove(OnRoleMove);
            roleRqs.RegistReq_RoleRotate(OnRoleRotate);
            roleRqs.RegistReq_Jump(OnRoleJump);
            roleRqs.RegistReq_BattleRoleSpawn(OnRoleSpawn);

            var itemRqs = battleFacades.Network.ItemReqAndRes;
            itemRqs.RegistReq_ItemPickUp(OnItemPickUp);

        }

        public void Tick(float fixedDeltaTime)
        {
            if (!hasBattleBegin)
            {
                return;
            }

            if (hasBattleBegin)
            {
                SpawBattleScene();
            }

            var curFieldEntity = serverFacades.BattleFacades.Repo.FiledRepo.CurFieldEntity;
            if (curFieldEntity == null)
            {
                return;
            }

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

        #region [Tick Request]

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
            ConnIDList.ForEach((connId) =>
            {
                long key = GetCurFrameKey(connId);

                if (roleSpawnMsgDic.TryGetValue(key, out var msg))
                {
                    if (msg == null)
                    {
                        return;
                    }

                    roleSpawnMsgDic[key] = null;

                    var battleFacades = serverFacades.BattleFacades;
                    var weaponRepo = battleFacades.Repo.WeaponRepo;
                    var roleRepo = battleFacades.Repo.RoleLogicRepo;
                    var entityID = roleRepo.Size;

                    // 服务器逻辑
                    var roleEntity = battleFacades.Domain.RoleDomain.SpawnRoleLogic(entityID);
                    roleEntity.SetConnId(connId);
                    var rqs = serverFacades.Network.RoleReqAndRes;
                    rqs.SendRes_BattleRoleSpawn(connId, entityID, msg.controlType);
                    Debug.Log($"服务器逻辑[生成角色] serveFrame:{ServerFrame} entityId:{entityID} controlType {((ControlType)msg.controlType).ToString()}");

                    var itemRqs = serverFacades.Network.ItemReqAndRes;
                    itemRqs.SendRes_ItemSpawn(connId, ServerFrame, serverFacades.ItemTypeByteList, serverFacades.SubTypeList, serverFacades.EntityIDList);
                }
            });
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

        #endregion

        #endregion

        #region [Network]

        #region [Role]

        void OnRoleMove(int connId, FrameRoleMoveReqMsg msg)
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

        void OnRoleJump(int connId, FrameRollReqMsg msg)
        {
            lock (rollOptMsgDic)
            {
                long key = GetCurFrameKey(connId);
                Debug.Log($"OnRoleJump ServeFrame {ServerFrame} key {key}");
                if (!rollOptMsgDic.TryGetValue(key, out var opt))
                {
                    rollOptMsgDic[key] = msg;
                }

            }
        }

        void OnRoleRotate(int connId, FrameRoleRotateReqMsg msg)
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

        void OnRoleSpawn(int connId, FrameBattleRoleSpawnReqMsg msg)
        {
            lock (roleSpawnMsgDic)
            {
                long key = GetCurFrameKey(connId);

                Debug.Log($"[战斗Controller] 战斗角色生成请求 key:{key}");
                if (!roleSpawnMsgDic.TryGetValue(key, out var _))
                {
                    roleSpawnMsgDic[key] = msg;
                }
            }
        }
        #endregion

        #region [Item]

        void OnItemPickUp(int connId, FrameItemPickReqMsg msg)
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

        #region [Scene Spawn Method]

        async void SpawBattleScene()
        {
            if (hasSpawnBegin)
            {
                return;
            }
            hasSpawnBegin = true;

            // Load Scene And Spawn Field
            var domain = serverFacades.BattleFacades.Domain;
            var fieldEntity = await domain.SceneDomain.SpawnGameFightScene();
            fieldEntity.SetEntityId(1);
            var fieldEntityRepo = serverFacades.BattleFacades.Repo.FiledRepo;
            fieldEntityRepo.Add(fieldEntity);
            fieldEntityRepo.SetPhysicsScene(fieldEntity.gameObject.scene.GetPhysicsScene());

            // 生成资源
            GenerateRandomAssetData(fieldEntity, out var assetPointEntities);
            InitAllAssetRepo(assetPointEntities);

        }

        void InitAllAssetRepo(AssetPointEntity[] assetPointEntities)
        {
            var entityTypeList = serverFacades.EntityTypeList;
            var entityIDList = serverFacades.EntityIDList;
            var itemTypeByteList = serverFacades.ItemTypeByteList;
            var subTypeList = serverFacades.SubTypeList;

            int count = entityTypeList.Count;
            Debug.Log($"物件资源开始生成[数量:{count}]----------------------------------------------------");

            var battleFacades = serverFacades.BattleFacades;
            for (int i = 0; i < entityTypeList.Count; i++)
            {
                var entityType = entityTypeList[i];
                var subtype = subTypeList[i];
                var parent = assetPointEntities[i];

                // - 获取资源ID
                var idService = battleFacades.IDService;
                var entityID = idService.GetAutoIDByEntityType(entityType);

                // - 生成资源
                var itemDomain = battleFacades.Domain.ItemDomain;
                itemDomain.SpawnItem(entityType, subtype, entityID, parent.transform);

                // - 记录
                entityIDList.Add(entityID);
                itemTypeByteList.Add((byte)entityType);
            }

            Debug.Log($"物件资源生成完毕******************************************************");
        }

        void GenerateRandomAssetData(FieldEntity fieldEntity, out AssetPointEntity[] assetPointEntities)
        {
            assetPointEntities = fieldEntity.transform.GetComponentsInChildren<AssetPointEntity>();
            for (int i = 0; i < assetPointEntities.Length; i++)
            {
                var assetPoint = assetPointEntities[i];
                AssetGenProbability[] itemGenProbabilities = assetPoint.itemGenProbabilityArray;
                float totalWeight = 0;
                for (int j = 0; j < itemGenProbabilities.Length; j++) totalWeight += itemGenProbabilities[j].weight;
                float lRange = 0;
                float rRange = 0;
                float randomNumber = Random.Range(0f, 1f);
                for (int j = 0; j < itemGenProbabilities.Length; j++)
                {
                    AssetGenProbability igp = itemGenProbabilities[j];
                    if (igp.weight <= 0) continue;
                    rRange = lRange + igp.weight / totalWeight;
                    if (randomNumber >= lRange && randomNumber < rRange)
                    {
                        serverFacades.EntityTypeList.Add(igp.entityType);
                        serverFacades.SubTypeList.Add(igp.subType);
                        break;
                    }
                    lRange = rRange;
                }
            }
        }
        #endregion

        #endregion

        #region [Private Func]

        long GetCurFrameKey(int connId)
        {
            long key = (long)ServerFrame << 32;
            key |= (long)connId;
            return key;
        }

        #endregion

    }


}
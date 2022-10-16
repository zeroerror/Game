using System.Collections.Generic;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Infrastructure.Generic;
using Game.Client.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.EventCenter;
using Game.Server.Bussiness.EventCenter;
using Game.Server.Bussiness.BattleBussiness.Network;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;
using System.Threading.Tasks;
using Game.Client.Bussiness;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleController
    {
        BattleServerFacades battleServerFacades;
        float fixedDeltaTime;  //0.03f

        // Scene Spawn Trigger
        bool hasBattleBegin;
        bool hasSpawnBegin;

        // NetWork Info
        public int ServeFrame => battleServerFacades.Network.ServeFrame;
        List<int> ConnIDList => battleServerFacades.Network.connIdList;

        // ====== 心跳 ======
        // - 所有客户端心跳帧
        Dictionary<long, BattleHeartbeatReqMsg> heartbeatMsgDic;

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

        // ====== 地图生成资源数据 ======
        ushort[] entityIdArray;
        byte[] itemTypeByteArray;
        List<byte> subTypeList = new List<byte>();

        public BattleController()
        {
            ServerNetworkEventCenter.battleSerConnect += ((connId) =>
            {
                ConnIDList.Add(connId); //添加至连接名单
                hasBattleBegin = true;
            });

            heartbeatMsgDic = new Dictionary<long, BattleHeartbeatReqMsg>();

            roleSpawnMsgDic = new Dictionary<long, FrameBattleRoleSpawnReqMsg>();
            roleMoveMsgDic = new Dictionary<long, FrameRoleMoveReqMsg>();
            roleRotateMsgDic = new Dictionary<long, FrameRoleRotateReqMsg>();
            rollOptMsgDic = new Dictionary<long, FrameRollReqMsg>();

            itemPickUpMsgDic = new Dictionary<long, FrameItemPickReqMsg>();
        }

        public void Inject(BattleServerFacades battleFacades, float fixedDeltaTime)
        {
            this.battleServerFacades = battleFacades;
            this.fixedDeltaTime = fixedDeltaTime;

            var battleRqs = battleFacades.Network.BattleReqAndRes;

            var roleRqs = battleFacades.Network.RoleReqAndRes;
            roleRqs.RegistReq_RoleMove(OnRoleMove);
            roleRqs.RegistReq_RoleRotate(OnRoleRotate);

            roleRqs.RegistReq_Jump(OnRoleJump);
            roleRqs.RegistReq_BattleRoleSpawn(OnRoleSpawn);

            var itemRqs = battleFacades.Network.ItemReqAndRes;
            itemRqs.RegistReq_ItemPickUp(OnItemPickUp);

        }

        public void Tick()
        {
            if (hasBattleBegin)
            {
                SpawBattleScene();
            }

            var CurFieldEntity = battleServerFacades.BattleFacades.Repo.FiledRepo.CurFieldEntity;
            if (CurFieldEntity == null)
            {
                return;
            }
            // ====== Bullet
            Tick_BulletLifeCycle();

            // ====== Role
            Tick_WRoleSpawn();
            Tick_RoleRollOpt();
            Tick_RoleMoveRotateOpt();
            ApplyAllRoleState();

            // ====== Item
            Tick_ItemPickUp();

            // ====== Broadcast
            BroadcastAllRoleState();
        }

        #region [Tick Request]

        #region [Role]

        void BroadcastAllRoleState()
        {
            var roleRqs = battleServerFacades.Network.RoleReqAndRes;
            var roleRepo = battleServerFacades.BattleFacades.Repo.RoleRepo;
            ConnIDList.ForEach((connID) =>
            {
                roleRepo.Foreach((role) =>
                {
                    roleRqs.SendUpdate_WRoleState(connID, role);
                });
            });
        }

        void Tick_WRoleSpawn()
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

                    var clientFacades = battleServerFacades.BattleFacades;
                    var repo = clientFacades.Repo;
                    var fieldEntity = repo.FiledRepo.Get(1);
                    var roleRqs = battleServerFacades.Network.RoleReqAndRes;
                    var roleRepo = repo.RoleRepo;
                    var itemRqs = battleServerFacades.Network.ItemReqAndRes;
                    var weaponRqs = battleServerFacades.Network.WeaponReqAndRes;
                    var weaponRepo = repo.WeaponRepo;
                    var wrid = roleRepo.Size;

                    // 服务器逻辑
                    var roleEntity = clientFacades.Domain.RoleDomain.SpawnRoleLogic(fieldEntity.transform);
                    roleEntity.Ctor();
                    roleEntity.IDComponent.SetEntityId(wrid);
                    roleEntity.SetConnId(connId);
                    roleRepo.Add(roleEntity);
                    Debug.Log($"服务器逻辑[生成角色] serveFrame:{ServeFrame} wRid:{wrid} 位置:{roleEntity.MoveComponent.Position}");

                    itemRqs.SendRes_ItemSpawn(connId, ServeFrame, itemTypeByteArray, subTypeList.ToArray(), entityIdArray);
                }
            });
        }

        void Tick_RoleMoveRotateOpt()
        {
            ConnIDList.ForEach((connId) =>
            {
                long key = GetCurFrameKey(connId);

                var roleRepo = battleServerFacades.BattleFacades.Repo.RoleRepo;
                var rqs = battleServerFacades.Network.RoleReqAndRes;

                if (roleMoveMsgDic.TryGetValue(key, out var msg))
                {
                    roleMoveMsgDic[key] = null;

                    var realMsg = msg.msg;

                    var rid = (byte)(realMsg >> 48);
                    var role = roleRepo.GetByEntityId(rid);

                    // ------------移动 -> Input
                    Vector3 dir = new Vector3((short)(ushort)(realMsg >> 32) / 100f, (short)(ushort)(realMsg >> 16) / 100f, (short)(ushort)realMsg / 100f);
                    role.InputComponent.SetMoveDir(dir);
                }

                if (roleRotateMsgDic.TryGetValue(key, out var rotateMsg))
                {
                    // ------------转向 -> Input（基于客户端鉴权的同步） 
                    var realMsg = rotateMsg.msg;
                    var rid = (byte)(realMsg >> 48);
                    var role = roleRepo.GetByEntityId(rid);
                    Vector3 eulerAngle = new Vector3((short)(realMsg >> 32), (short)(realMsg >> 16), (short)realMsg);
                    role.InputComponent.SetRotEuler(eulerAngle);
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
                    var roleRepo = battleServerFacades.BattleFacades.Repo.RoleRepo;
                    var role = roleRepo.GetByEntityId(wRid);
                    var rqs = battleServerFacades.Network.RoleReqAndRes;
                    Vector3 dir = new Vector3(msg.dirX / 10000f, msg.dirY / 10000f, msg.dirZ / 10000f);
                    role.InputComponent.SetRollDir(dir);
                }
            });

        }

        void ApplyAllRoleState()
        {
            var roleStateDomain = battleServerFacades.BattleFacades.Domain.RoleStateDomain;
            roleStateDomain.ApplyAllRoleState();
        }

        #endregion

        #region [Bullet]

        void Tick_BulletLifeCycle()
        {
            Tick_DeadLifeBulletTearDown();
            Tick_BulletHitRole();
            Tick_BulletHitField();
            Tick_ActiveHookerDraging();
        }

        void Tick_DeadLifeBulletTearDown()
        {
            var bulletDomain = battleServerFacades.BattleFacades.Domain.BulletDomain;
            var tearDownList = bulletDomain.Tick_BulletLife(NetworkConfig.FIXED_DELTA_TIME);
            if (tearDownList.Count == 0) return;

            tearDownList.ForEach((bulletEntity) =>
            {

                var bulletType = bulletEntity.BulletType;

                if (bulletType == BulletType.DefaultBullet)
                {
                    bulletEntity.TearDown();
                }

                if (bulletEntity is GrenadeEntity grenadeEntity)
                {
                    Debug.Log("爆炸");
                    var bulletDomain = battleServerFacades.BattleFacades.Domain.BulletDomain;
                    bulletDomain.GrenadeExplode(grenadeEntity, fixedDeltaTime);
                }

                if (bulletEntity is HookerEntity hookerEntity)
                {
                    hookerEntity.TearDown();
                }

                var bulletRepo = battleServerFacades.BattleFacades.Repo.BulletRepo;
                bulletRepo.TryRemove(bulletEntity);

                var bulletRqs = battleServerFacades.Network.BulletReqAndRes;
                ConnIDList.ForEach((connId) =>
                {
                    // 广播子弹销毁消息
                    bulletRqs.SendRes_BulletLifeFrameOver(connId, bulletEntity);
                });

            });
        }

        void Tick_BulletHitField()
        {
            Transform field = null;
            var bulletDomain = battleServerFacades.BattleFacades.Domain.BulletDomain;
            var bulletRepo = battleServerFacades.BattleFacades.Repo.BulletRepo;
            var bulletRqs = battleServerFacades.Network.BulletReqAndRes;
            var hitFieldList = bulletDomain.Tick_BulletHitField(fixedDeltaTime);

            hitFieldList.ForEach((hitFieldModel) =>
            {
                var bulletIDC = hitFieldModel.hitter;
                var bullet = bulletRepo.GetByBulletId(bulletIDC.EntityId);

                ConnIDList.ForEach((connId) =>
                {
                    bulletRqs.SendRes_BulletHitField(connId, bullet);
                });
                field = hitFieldModel.fieldCE.Collider.transform;
            });
        }

        void Tick_BulletHitRole()
        {
            var bulletDomain = battleServerFacades.BattleFacades.Domain.BulletDomain;
            var bulletRqs = battleServerFacades.Network.BulletReqAndRes;
            var hitRoleList = bulletDomain.Tick_BulletHitRole(fixedDeltaTime);

            ConnIDList.ForEach((connId) =>
            {
                hitRoleList.ForEach((attackModel) =>
                {
                    bulletRqs.SendRes_BulletHitRole(connId, attackModel.attackerIDC.EntityId, attackModel.victimIDC.EntityId);
                });
            });
        }

        void Tick_ActiveHookerDraging()
        {
            var bulletDomain = battleServerFacades.BattleFacades.Domain.BulletDomain;
            bulletDomain.Tick_ActiveHookerDraging(fixedDeltaTime);
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
                    var repo = battleServerFacades.BattleFacades.Repo;
                    var roleRepo = repo.RoleRepo;
                    var role = roleRepo.GetByEntityId(msg.wRid);
                    ItemType itemType = (ItemType)msg.itemType;

                    var itemDomain = battleServerFacades.BattleFacades.Domain.ItemDomain;
                    if (itemDomain.TryPickUpItem(itemType, msg.entityId, repo, role))
                    {
                        var rqs = battleServerFacades.Network.ItemReqAndRes;
                        ConnIDList.ForEach((connId) =>
                        {
                            rqs.SendRes_ItemPickUp(connId, ServeFrame, msg.wRid, itemType, msg.entityId);
                        });
                    }
                    else
                    {
                        Debug.Log($"{itemType.ToString()}物品拾取失败");
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
            var domain = battleServerFacades.BattleFacades.Domain;
            var fieldEntity = await domain.SpawnDomain.SpawnGameFightScene();
            fieldEntity.SetFieldId(1);
            var fieldEntityRepo = battleServerFacades.BattleFacades.Repo.FiledRepo;
            fieldEntityRepo.Add(fieldEntity);
            fieldEntityRepo.SetPhysicsScene(fieldEntity.gameObject.scene.GetPhysicsScene());

            // 生成资源
            GenerateRandomAssetData(fieldEntity, out List<ItemType> itemTypeList, out AssetPointEntity[] assetPointEntities);
            InitAllAssetRepo(itemTypeList, assetPointEntities);
        }

        void InitAllAssetRepo(List<ItemType> itemTypeList, AssetPointEntity[] assetPointEntities)
        {
            int count = itemTypeList.Count;
            entityIdArray = new ushort[count];
            itemTypeByteArray = new byte[count];
            Debug.Log($"物件资源开始生成[数量:{count}]----------------------------------------------------");
            int index = 0;
            itemTypeList.ForEach((itemType) =>
            {
                var parent = assetPointEntities[index];
                var subtype = subTypeList[index];
                itemTypeByteArray[index] = (byte)itemType;
                // 生成武器资源
                var itemDomain = battleServerFacades.BattleFacades.Domain.ItemDomain;
                var item = itemDomain.SpawnItem(itemType, subtype);

                ushort entityId = 0;
                switch (itemType)
                {
                    case ItemType.Default:
                        break;
                    case ItemType.Weapon:
                        var weaponEntity = item.GetComponent<WeaponEntity>();
                        var weaponRepo = battleServerFacades.BattleFacades.Repo.WeaponRepo;
                        entityId = weaponRepo.WeaponIdAutoIncreaseId;
                        weaponEntity.Ctor();
                        weaponEntity.SetEntityId(entityId);
                        weaponRepo.Add(weaponEntity);
                        Debug.Log($"生成武器资源:{entityId}");

                        entityIdArray[index] = entityId;    //记录

                        break;
                    case ItemType.BulletPack:
                        var bulletPackEntity = item.GetComponent<BulletPackEntity>();
                        var bulletPackRepo = battleServerFacades.BattleFacades.Repo.BulletPackRepo;
                        entityId = bulletPackRepo.bulletPackAutoIncreaseId;
                        bulletPackEntity.Ctor();
                        bulletPackEntity.SetEntityId(entityId);
                        bulletPackRepo.Add(bulletPackEntity);
                        bulletPackRepo.bulletPackAutoIncreaseId++;
                        Debug.Log($"生成子弹包资源:{entityId}");

                        entityIdArray[index] = entityId;    //记录

                        break;
                    case ItemType.Pill:
                        break;

                }

                item.transform.SetParent(parent.transform);
                item.transform.localPosition = Vector3.zero;
                item.name += entityId;

                index++;
            });

            Debug.Log($"物件资源生成完毕******************************************************");
        }

        void GenerateRandomAssetData(FieldEntity fieldEntity, out List<ItemType> itemTypeList, out AssetPointEntity[] assetPointEntities)
        {
            itemTypeList = new List<ItemType>();
            assetPointEntities = fieldEntity.transform.GetComponentsInChildren<AssetPointEntity>();
            for (int i = 0; i < assetPointEntities.Length; i++)
            {
                var assetPoint = assetPointEntities[i];
                ItemGenProbability[] itemGenProbabilities = assetPoint.itemGenProbabilityArray;
                float totalWeight = 0;
                for (int j = 0; j < itemGenProbabilities.Length; j++) totalWeight += itemGenProbabilities[j].weight;
                float lRange = 0;
                float rRange = 0;
                float randomNumber = Random.Range(0f, 1f);
                for (int j = 0; j < itemGenProbabilities.Length; j++)
                {
                    ItemGenProbability igp = itemGenProbabilities[j];
                    if (igp.weight <= 0) continue;
                    rRange = lRange + igp.weight / totalWeight;
                    if (randomNumber >= lRange && randomNumber < rRange)
                    {
                        itemTypeList.Add(igp.itemType);
                        subTypeList.Add(igp.subType);
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
            long key = (long)ServeFrame << 32;
            key |= (long)connId;
            return key;
        }

        #endregion

    }


}
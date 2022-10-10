using System.Collections.Generic;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Infrastructure.Generic;
using Game.Client.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.EventCenter;
using Game.Server.Bussiness.EventCenter;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleController
    {
        BattleFacades battleFacades;
        float fixedDeltaTime;  //0.03f

        // Scene Spawn Trigger
        bool sceneSpawnTrigger;
        bool isSceneSpawn;

        // NetWork Info
        public int ServeFrame => battleFacades.Network.ServeFrame;
        List<int> ConnIdList => battleFacades.Network.connIdList;

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
        Dictionary<long, FrameJumpReqMsg> jumpOptMsgDic;

        // ====== 子弹 ======
        // - 所有子弹生成帧
        Dictionary<long, FrameBulletSpawnReqMsg> bulletSpawnMsgDic;
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
            });

            heartbeatMsgDic = new Dictionary<long, BattleHeartbeatReqMsg>();

            roleSpawnMsgDic = new Dictionary<long, FrameBattleRoleSpawnReqMsg>();
            roleMoveMsgDic = new Dictionary<long, FrameRoleMoveReqMsg>();
            roleRotateMsgDic = new Dictionary<long, FrameRoleRotateReqMsg>();
            jumpOptMsgDic = new Dictionary<long, FrameJumpReqMsg>();

            bulletSpawnMsgDic = new Dictionary<long, FrameBulletSpawnReqMsg>();

            itemPickUpMsgDic = new Dictionary<long, FrameItemPickReqMsg>();
        }

        public void Inject(BattleFacades battleFacades, float fixedDeltaTime)
        {
            this.battleFacades = battleFacades;
            this.fixedDeltaTime = fixedDeltaTime;

            var battleRqs = battleFacades.Network.BattleReqAndRes;
            battleRqs.RegistReq_HeartBeat(OnHeartbeat);

            var roleRqs = battleFacades.Network.BattleRoleReqAndRes;
            roleRqs.RegistReq_RoleMove(OnRoleMove);
            roleRqs.RegistReq_RoleRotate(OnRoleRotate);

            roleRqs.RegistReq_Jump(OnRoleJump);
            roleRqs.RegistReq_BattleRoleSpawn(OnRoleSpawn);

            var bulletRqs = battleFacades.Network.BulletReqAndRes;
            bulletRqs.RegistReq_BulletSpawn(OnBulletSpawn);

            var itemRqs = battleFacades.Network.ItemReqAndRes;
            itemRqs.RegistReq_ItemPickUp(OnItemPickUp);

        }

        public void Tick()
        {
            // Tick的过滤条件
            if (sceneSpawnTrigger && !isSceneSpawn)
            {
                SpawBattleChooseScene();
                sceneSpawnTrigger = false;
            }
            if (!isSceneSpawn) return;

            // ====== Life
            Tick_BulletLife();
            Tick_ActiveHookersBehaviour();

            // Client Request
            Tick_WRoleSpawn();
            Tick_BulletSpawn();

            Tick_ItemPickUp();

            Tick_JumpOpt();
            Tick_MoveAndRotateOpt();

            Tick_Heartbeat();
        }

        #region [Tick Request]

        #region [Role]
        void Tick_WRoleSpawn()
        {
            ConnIdList.ForEach((connId) =>
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (roleSpawnMsgDic.TryGetValue(key, out var msg))
                {
                    var clientFacades = battleFacades.ClientBattleFacades;
                    var repo = clientFacades.Repo;
                    var fieldEntity = repo.FiledRepo.Get(1);
                    var roleRqs = battleFacades.Network.BattleRoleReqAndRes;
                    var roleRepo = repo.RoleRepo;
                    var itemRqs = battleFacades.Network.ItemReqAndRes;
                    var weaponRqs = battleFacades.Network.WeaponReqAndRes;
                    var weaponRepo = repo.WeaponRepo;
                    var wrid = roleRepo.Size;

                    // 服务器逻辑
                    var roleEntity = clientFacades.Domain.BattleRoleDomain.SpawnBattleRoleLogic(fieldEntity.transform);
                    roleEntity.Ctor();
                    roleEntity.SetEntityId(wrid);
                    roleEntity.SetConnId(connId);
                    Debug.Log($"服务器逻辑[生成角色] serveFrame:{ServeFrame} wRid:{wrid} 位置:{roleEntity.MoveComponent.CurPos}");

                    // ===== TODO:同步所有信息给请求者
                    var allRole = roleRepo.GetAll();
                    for (int i = 0; i < allRole.Length; i++)
                    {
                        var otherRole = allRole[i];
                        roleRqs.SendUpdate_WRoleState(connId, otherRole);
                    }

                    itemRqs.SendRes_ItemSpawn(connId, ServeFrame, itemTypeByteArray, subTypeList.ToArray(), entityIdArray);

                    // ====== 广播请求者创建的角色给其他人
                    ConnIdList.ForEach((otherConnId) =>
                    {
                        if (otherConnId != connId)
                        {
                            roleRqs.SendUpdate_WRoleState(otherConnId, roleEntity);
                        }
                    });

                    // ====== 回复请求者创建的角色
                    roleRqs.SendUpdate_WRoleState(connId, roleEntity);

                    roleRepo.Add(roleEntity);
                }
            });
        }

        void Tick_RoleStateIdle(int nextFrame)
        {
            //人物静止和运动 2个状态
            var BattleRoleRepo = battleFacades.ClientBattleFacades.Repo.RoleRepo;
            BattleRoleRepo.Foreach((roleEntity) =>
            {
                if (roleEntity.IsIdle() && roleEntity.RoleState != RoleState.Normal)
                {
                    roleEntity.SetRoleState(RoleState.Normal);

                    var rqs = battleFacades.Network.BattleRoleReqAndRes;
                    ConnIdList.ForEach((connId) =>
                    {
                        rqs.SendUpdate_WRoleState(connId, roleEntity);
                    });
                }
            });
        }

        void Tick_MoveAndRotateOpt()
        {
            ConnIdList.ForEach((connId) =>
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;
                var roleRepo = battleFacades.ClientBattleFacades.Repo.RoleRepo;
                var rqs = battleFacades.Network.BattleRoleReqAndRes;

                if (roleMoveMsgDic.TryGetValue(key, out var moveMsg))
                {
                    var realMsg = moveMsg.msg;

                    var rid = (byte)(realMsg >> 48);
                    var role = roleRepo.GetByEntityId(rid);

                    // ------------移动
                    Vector3 dir = new Vector3((short)(ushort)(realMsg >> 32) / 100f, (short)(ushort)(realMsg >> 16) / 100f, (short)(ushort)realMsg / 100f);

                    //服务器逻辑
                    role.MoveComponent.ActivateMoveVelocity(dir);

                    //发送状态同步帧
                    if (role.RoleState != RoleState.Hooking) role.SetRoleState(RoleState.Move);
                    ConnIdList.ForEach((otherConnId) =>
                    {
                        rqs.SendUpdate_WRoleState(otherConnId, role);
                    });
                }

                if (roleRotateMsgDic.TryGetValue(key, out var rotateMsg))
                {
                    // ------------转向（基于客户端鉴权的同步）
                    var realMsg = rotateMsg.msg;
                    var rid = (byte)(realMsg >> 48);
                    var role = roleRepo.GetByEntityId(rid);
                    Vector3 eulerAngle = new Vector3((short)(realMsg >> 32), (short)(realMsg >> 16), (short)realMsg);
                    role.MoveComponent.SetEulerAngle(eulerAngle);
                    //发送状态同步帧
                    ConnIdList.ForEach((otherConnId) =>
                    {
                        rqs.SendUpdate_WRoleState(otherConnId, role);
                    });
                }

            });

        }

        void Tick_JumpOpt()
        {
            ConnIdList.ForEach((connId) =>
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;
                if (jumpOptMsgDic.TryGetValue(key, out var opt))
                {
                    var wRid = opt.wRid;
                    var roleRepo = battleFacades.ClientBattleFacades.Repo.RoleRepo;
                    var roleEntity = roleRepo.GetByEntityId(wRid);
                    var rqs = battleFacades.Network.BattleRoleReqAndRes;

                    //服务器逻辑Jump
                    if (roleEntity.MoveComponent.TryJump())
                    {
                        if (roleEntity.RoleState != RoleState.Hooking) roleEntity.SetRoleState(RoleState.Jump);
                        //发送状态同步帧
                        ConnIdList.ForEach((connId) =>
                        {
                            rqs.SendUpdate_WRoleState(connId, roleEntity);
                        });
                    }
                }

            });

        }
        #endregion

        #region [Bullet]
        void Tick_BulletSpawn()
        {

            ConnIdList.ForEach((connId) =>
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (bulletSpawnMsgDic.TryGetValue(key, out var msg))
                {
                    var bulletTypeByte = msg.bulletType;
                    byte wRid = msg.wRid;
                    float targetPosX = msg.targetPosX / 10000f;
                    float targetPosY = msg.targetPosY / 10000f;
                    float targetPosZ = msg.targetPosZ / 10000f;
                    Vector3 targetPos = new Vector3(targetPosX, targetPosY, targetPosZ);
                    var roleEntity = battleFacades.ClientBattleFacades.Repo.RoleRepo.GetByEntityId(msg.wRid);
                    var moveComponent = roleEntity.MoveComponent;
                    var shootStartPoint = roleEntity.ShootPointPos;
                    Vector3 shootDir = targetPos - shootStartPoint;
                    shootDir.Normalize();

                    // 服务器逻辑
                    var bulletType = (BulletType)bulletTypeByte;
                    var clientFacades = battleFacades.ClientBattleFacades;
                    var fieldEntity = clientFacades.Repo.FiledRepo.Get(1);

                    var bulletEntity = clientFacades.Domain.BulletDomain.SpawnBullet(fieldEntity.transform, bulletType);
                    var bulletRepo = clientFacades.Repo.BulletRepo;
                    var bulletId = bulletRepo.BulletCount;
                    bulletEntity.MoveComponent.SetCurPos(shootStartPoint);
                    bulletEntity.MoveComponent.SetForward(shootDir);
                    bulletEntity.MoveComponent.ActivateMoveVelocity(shootDir);
                    switch (bulletType)
                    {
                        case BulletType.DefaultBullet:
                            break;
                        case BulletType.Grenade:
                            break;
                        case BulletType.Hooker:
                            var hookerEntity = (HookerEntity)bulletEntity;
                            hookerEntity.SetMasterWRid(roleEntity.EntityId);
                            hookerEntity.SetMasterGrabPoint(roleEntity.transform);
                            break;
                    }
                    bulletEntity.SetMasterId(wRid);
                    bulletEntity.SetEntityId(bulletId);
                    bulletEntity.gameObject.SetActive(true);
                    bulletRepo.Add(bulletEntity);
                    Debug.Log($"服务器逻辑[生成子弹] serveFrame {ServeFrame} connId {connId}:  bulletType:{bulletTypeByte.ToString()} bulletId:{bulletId}  MasterWRid:{wRid}  起点：{shootStartPoint} 终点：{targetPos} 飞行方向:{shootDir}");

                    var rqs = battleFacades.Network.BulletReqAndRes;
                    ConnIdList.ForEach((otherConnId) =>
                    {
                        rqs.SendRes_BulletSpawn(otherConnId, bulletType, bulletId, wRid, shootDir);
                    });
                }
            });

        }

        void Tick_BulletLife()
        {
            var tearDownList = battleFacades.ClientBattleFacades.Domain.BulletDomain.Tick_BulletLife(NetworkConfig.FIXED_DELTA_TIME);
            if (tearDownList.Count == 0) return;

            tearDownList.ForEach((bulletEntity) =>
            {

                Queue<BattleRoleLogicEntity> effectRoleQueue = new Queue<BattleRoleLogicEntity>();
                var bulletType = bulletEntity.BulletType;
                if (bulletType == BulletType.DefaultBullet)
                {
                    bulletEntity.TearDown();
                }
                else if (bulletEntity is GrenadeEntity grenadeEntity)
                {
                    Debug.Log("爆炸");
                    grenadeEntity.TearDown();
                    var roleRepo = battleFacades.ClientBattleFacades.Repo.RoleRepo;
                    roleRepo.Foreach((role) =>
                    {
                        var dis = Vector3.Distance(role.MoveComponent.CurPos, bulletEntity.MoveComponent.CurPos);
                        if (dis < 7f)
                        {
                            var dir = role.MoveComponent.CurPos - bulletEntity.MoveComponent.CurPos;
                            var extraV = dir.normalized * 10f;
                            role.MoveComponent.AddExtraVelocity(extraV);
                            role.MoveComponent.Tick_Rigidbody(fixedDeltaTime);
                            role.SetRoleState(RoleState.Move);
                            effectRoleQueue.Enqueue(role);
                        }
                    });
                }
                else if (bulletEntity is HookerEntity hookerEntity)
                {
                    hookerEntity.TearDown();
                }

                var bulletRepo = battleFacades.ClientBattleFacades.Repo.BulletRepo;
                bulletRepo.TryRemove(bulletEntity);

                var bulletRqs = battleFacades.Network.BulletReqAndRes;
                var roleRqs = battleFacades.Network.BattleRoleReqAndRes;
                ConnIdList.ForEach((connId) =>
                {
                    // 广播子弹销毁消息
                    bulletRqs.SendRes_BulletTearDown(connId, bulletType, bulletEntity.MasterId, bulletEntity.EntityId, bulletEntity.MoveComponent.CurPos);
                });
                while (effectRoleQueue.TryDequeue(out var role))
                {
                    Debug.Log($"角色击飞发送");
                    ConnIdList.ForEach((connId) =>
                    {
                        // 广播被影响角色的最新状态消息
                        roleRqs.SendUpdate_WRoleState(connId, role);
                    });
                }

            });
        }

        void Tick_ActiveHookersBehaviour()
        {
            var activeHookers = battleFacades.ClientBattleFacades.Domain.BulletDomain.GetActiveHookerList();
            List<BattleRoleLogicEntity> roleList = new List<BattleRoleLogicEntity>();
            var rqs = battleFacades.Network.BattleRoleReqAndRes;
            activeHookers.ForEach((hooker) =>
            {
                var master = battleFacades.ClientBattleFacades.Repo.RoleRepo.GetByEntityId(hooker.MasterId);
                if (!hooker.TickHooker(out float force))
                {
                    master.SetRoleState(RoleState.Normal);
                    //发送爪钩断开后的角色状态帧
                    ConnIdList.ForEach((connId) =>
                    {
                        rqs.SendUpdate_WRoleState(connId, master);
                    });
                    return;
                }

                var masterMC = master.MoveComponent;
                var hookerEntityMC = hooker.MoveComponent;
                var dir = hookerEntityMC.CurPos - masterMC.CurPos;
                var dis = Vector3.Distance(hookerEntityMC.CurPos, masterMC.CurPos);
                dir.Normalize();
                var v = dir * force * fixedDeltaTime;
                masterMC.AddExtraVelocity(v);
                master.SetRoleState(RoleState.Hooking);

                if (!roleList.Contains(master)) roleList.Add(master);
            });

            roleList.ForEach((master) =>
            {
                //发送爪钩作用力后的角色状态帧
                ConnIdList.ForEach((connId) =>
                {
                    rqs.SendUpdate_WRoleState(connId, master);
                });
            });

        }
        #endregion

        #region [Item]
        void Tick_ItemPickUp()
        {

            ConnIdList.ForEach((connId) =>
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (itemPickUpMsgDic.TryGetValue(key, out var msg))
                {
                    // TODO:Add judgement like 'Can He Pick It Up?'
                    var repo = battleFacades.ClientBattleFacades.Repo;
                    var roleRepo = repo.RoleRepo;
                    var role = roleRepo.GetByEntityId(msg.wRid);
                    ItemType itemType = (ItemType)msg.itemType;
                    var itemDomain = battleFacades.ClientBattleFacades.Domain.ItemDomain;
                    bool isPickUpSucceed = itemDomain.TryPickUpItem(itemType, msg.entityId, repo, role);
                    if (isPickUpSucceed)
                    {
                        var rqs = battleFacades.Network.ItemReqAndRes;
                        ConnIdList.ForEach((connId) =>
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

        void Tick_Heartbeat()
        {
            ConnIdList.ForEach((connId) =>
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (heartbeatMsgDic.TryGetValue(key, out var _))
                {
                    var rqs = battleFacades.Network.BattleReqAndRes;
                    rqs.SendRes_HeartBeat(connId);
                }
            });
        }

        #endregion

        #region [Network]

        #region [Role]
        void OnRoleMove(int connId, FrameRoleMoveReqMsg msg)
        {
            lock (roleMoveMsgDic)
            {

                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (!roleMoveMsgDic.TryGetValue(key, out var opt))
                {
                    roleMoveMsgDic[key] = msg;
                }

            }
        }

        void OnRoleJump(int connId, FrameJumpReqMsg msg)
        {
            lock (jumpOptMsgDic)
            {

                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (!jumpOptMsgDic.TryGetValue(key, out var opt))
                {
                    jumpOptMsgDic[key] = msg;
                }

            }
        }

        void OnRoleRotate(int connId, FrameRoleRotateReqMsg msg)
        {
            lock (roleMoveMsgDic)
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;
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
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                Debug.Log($"[战斗Controller] 战斗角色生成请求 key:{key}");
                if (!roleSpawnMsgDic.TryGetValue(key, out var _))
                {
                    roleSpawnMsgDic[key] = msg;
                }

                ConnIdList.Add(connId); //角色生成后添加至连接名单

                sceneSpawnTrigger = true;// 创建场景(First Time)

            }

        }
        #endregion

        #region [Bullet]
        void OnBulletSpawn(int connId, FrameBulletSpawnReqMsg msg)
        {
            lock (bulletSpawnMsgDic)
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (!bulletSpawnMsgDic.TryGetValue(key, out var _))
                {
                    bulletSpawnMsgDic[key] = msg;
                }
            }
        }
        #endregion

        #region [Item]
        void OnItemPickUp(int connId, FrameItemPickReqMsg msg)
        {
            lock (itemPickUpMsgDic)
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;
                if (!itemPickUpMsgDic.TryGetValue(key, out var _))
                {
                    itemPickUpMsgDic[key] = msg;
                }
            }
        }
        #endregion

        #region [Scene Spawn Method]
        async void SpawBattleChooseScene()
        {
            // Load Scene And Spawn Field
            var domain = battleFacades.ClientBattleFacades.Domain;
            var fieldEntity = await domain.BattleSpawnDomain.SpawnGameFightScene();
            fieldEntity.SetFieldId(1);
            var fieldEntityRepo = battleFacades.ClientBattleFacades.Repo.FiledRepo;
            fieldEntityRepo.Add(fieldEntity);
            fieldEntityRepo.SetPhysicsScene(fieldEntity.gameObject.scene.GetPhysicsScene());
            isSceneSpawn = true;

            // 生成场景资源，并回复客户端
            List<ItemType> itemTypeList = new List<ItemType>();
            AssetPointEntity[] assetPointEntities = fieldEntity.transform.GetComponentsInChildren<AssetPointEntity>();
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

            int count = itemTypeList.Count;
            entityIdArray = new ushort[count];
            itemTypeByteArray = new byte[count];
            Debug.Log($"服务器地图物件资源开始生成[数量:{count}]----------------------------------------------------");
            int index = 0;
            itemTypeList.ForEach((itemType) =>
            {
                var parent = assetPointEntities[index];
                var subtype = subTypeList[index];
                itemTypeByteArray[index] = (byte)itemType;
                // 生成武器资源
                var itemDomain = battleFacades.ClientBattleFacades.Domain.ItemDomain;
                var item = itemDomain.SpawnItem(itemType, subtype);

                ushort entityId = 0;
                switch (itemType)
                {
                    case ItemType.Default:
                        break;
                    case ItemType.Weapon:
                        var weaponEntity = item.GetComponent<WeaponEntity>();
                        var weaponRepo = battleFacades.ClientBattleFacades.Repo.WeaponRepo;
                        entityId = weaponRepo.WeaponIdAutoIncreaseId;
                        weaponEntity.Ctor();
                        weaponEntity.SetEntityId(entityId);
                        weaponRepo.Add(weaponEntity);
                        Debug.Log($"生成武器资源:{entityId}");
                        entityIdArray[index] = entityId;
                        break;
                    case ItemType.BulletPack:
                        var bulletPackEntity = item.GetComponent<BulletPackEntity>();
                        var bulletPackRepo = battleFacades.ClientBattleFacades.Repo.BulletPackRepo;
                        entityId = bulletPackRepo.bulletPackAutoIncreaseId;
                        bulletPackEntity.Ctor();
                        bulletPackEntity.SetEntityId(entityId);
                        bulletPackRepo.Add(bulletPackEntity);
                        Debug.Log($"生成子弹包资源:{entityId}");
                        entityIdArray[index] = entityId;
                        bulletPackRepo.bulletPackAutoIncreaseId++;
                        break;
                    case ItemType.Pill:
                        break;

                }

                item.transform.SetParent(parent.transform);
                item.transform.localPosition = Vector3.zero;
                item.name += entityId;

                index++;
            });

            Debug.Log($"地图物件资源生成完毕******************************************************");

            // var rqs = battleFacades.Network.ItemReqAndRes;
            // connIdList.ForEach((connId) =>
            // {
            //     rqs.SendRes_ItemSpawn(connId, serveFrame, itemTypeByteArray, subTypeList.ToArray(), entityIdArray);
            // });
        }
        #endregion

        void OnHeartbeat(int connId, BattleHeartbeatReqMsg msg)
        {
            lock (roleMoveMsgDic)
            {

                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (!heartbeatMsgDic.TryGetValue(key, out var _))
                {
                    heartbeatMsgDic[key] = msg;
                }

            }
        }

        #endregion

    }


}
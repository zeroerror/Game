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

        public int serveFrame => battleFacades.Network.ServeFrame;

        // 当前所有ConnId
        List<int> connIdList => battleFacades.Network.connIdList;

        // 记录所有操作帧
        Dictionary<long, FrameRoleMoveReqMsg> roleMoveMsgDic;
        Dictionary<long, FrameRoleRotateReqMsg> roleRotateMsgDic;

        // 移动记录所有跳跃帧
        struct FrameJumpReqMsgStruct
        {
            public int connId;
            public FrameJumpReqMsg msg;
        }
        Dictionary<int, Queue<FrameJumpReqMsgStruct>> jumpOptQueueDic;//TODO: --> Queue

        // 记录所有角色生成帧
        struct FrameWRoleSpawnReqMsgStruct
        {
            public int connId;
            public FrameBattleRoleSpawnReqMsg msg;
        }
        Dictionary<int, Queue<FrameWRoleSpawnReqMsgStruct>> wRoleSpawQueuenDic;//TODO: --> Queue

        // 记录所有子弹生成帧
        struct FrameBulletSpawnReqMsgStruct
        {
            public int connId;
            public FrameBulletSpawnReqMsg msg;
        }
        Dictionary<int, Queue<FrameBulletSpawnReqMsgStruct>> bulletSpawnQueueDic;   //TODO: --> 

        // 记录所有拾取物件帧
        struct FrameItemPickUpReqMsgStruct
        {
            public int connId;
            public FrameItemPickReqMsg msg;
        }
        Dictionary<int, Queue<FrameItemPickUpReqMsgStruct>> itemPickUpQueueDic;

        // 地图生成资源数据
        ushort[] entityIdArray;
        byte[] itemTypeByteArray;
        List<byte> subTypeList = new List<byte>();

        // 记录所有武器射击帧
        struct FrameWeaponShootReqMsgStruct
        {
            public int connId;
            public FrameWeaponShootReqMsg msg;
        }
        Dictionary<int, Queue<FrameWeaponShootReqMsgStruct>> weaponShootQueueDic;

        // 记录所有武器装弹帧
        struct FrameWeaponReloadReqMsgStruct
        {
            public int connId;
            public FrameWeaponReloadReqMsg msg;
        }
        Dictionary<int, Queue<FrameWeaponReloadReqMsgStruct>> weaponReloadQueueDic;

        // 记录所有武器丢弃帧
        struct FrameWeaponDropReqMsgStruct
        {
            public int connId;
            public FrameWeaponDropReqMsg msg;
        }
        Dictionary<int, Queue<FrameWeaponDropReqMsgStruct>> weaponDropQueueDic;

        bool sceneSpawnTrigger;
        bool isSceneSpawn;

        public BattleController()
        {
            ServerNetworkEventCenter.battleSerConnect += ((connId) =>
            {
            });

            roleMoveMsgDic = new Dictionary<long, FrameRoleMoveReqMsg>();
            roleRotateMsgDic = new Dictionary<long, FrameRoleRotateReqMsg>();

            jumpOptQueueDic = new Dictionary<int, Queue<FrameJumpReqMsgStruct>>();
            wRoleSpawQueuenDic = new Dictionary<int, Queue<FrameWRoleSpawnReqMsgStruct>>();
            bulletSpawnQueueDic = new Dictionary<int, Queue<FrameBulletSpawnReqMsgStruct>>();
            itemPickUpQueueDic = new Dictionary<int, Queue<FrameItemPickUpReqMsgStruct>>();
            weaponShootQueueDic = new Dictionary<int, Queue<FrameWeaponShootReqMsgStruct>>();
            weaponReloadQueueDic = new Dictionary<int, Queue<FrameWeaponReloadReqMsgStruct>>();
            weaponDropQueueDic = new Dictionary<int, Queue<FrameWeaponDropReqMsgStruct>>();
        }

        public void Inject(BattleFacades battleFacades, float fixedDeltaTime)
        {
            this.battleFacades = battleFacades;
            this.fixedDeltaTime = fixedDeltaTime;

            var roleRqs = battleFacades.Network.BattleRoleReqAndRes;
            roleRqs.RegistReq_RoleMove(OnRoleMove);
            roleRqs.RegistReq_RoleRotate(OnRoleRotate);

            roleRqs.RegistReq_Jump(OnRoleJump);
            roleRqs.RegistReq_BattleRoleSpawn(OnBattleRoleSpawn);

            var bulletRqs = battleFacades.Network.BulletReqAndRes;
            bulletRqs.RegistReq_BulletSpawn(OnBulletSpawn);

            var itemRqs = battleFacades.Network.ItemReqAndRes;
            itemRqs.RegistReq_ItemPickUp(OnItemPickUp);

            var weaponRqs = battleFacades.Network.WeaponReqAndRes;
            weaponRqs.RegistReq_WeaponShoot(OnWeaponShoot);
            weaponRqs.RegistReq_WeaponReload(OnWeaponReload);
            weaponRqs.RegistReq_WeaponDrop(OnWeaponDrop);

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
            Tick_WeaponShoot();
            Tick_WeaponReload();
            Tick_WeaponDrop();

            Tick_JumpOpt();
            Tick_MoveAndRotateOpt();

        }

        #region [Client Requst]

        #region [Role]
        void Tick_WRoleSpawn()
        {
            if (wRoleSpawQueuenDic.TryGetValue(serveFrame, out var queue))
            {
                while (queue.TryDequeue(out var msgStruct))
                {
                    var msg = msgStruct.msg;
                    var connId = msgStruct.connId;

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
                    Debug.Log($"服务器逻辑[生成角色] serveFrame:{serveFrame} wRid:{wrid} 位置:{roleEntity.MoveComponent.CurPos}");

                    // ===== TODO:同步所有信息给请求者
                    var allRole = roleRepo.GetAll();
                    for (int i = 0; i < allRole.Length; i++)
                    {
                        var otherRole = allRole[i];
                        roleRqs.SendUpdate_WRoleState(connId, otherRole);
                    }

                    itemRqs.SendRes_ItemSpawn(connId, serveFrame, itemTypeByteArray, subTypeList.ToArray(), entityIdArray);

                    // ====== 广播请求者创建的角色给其他人
                    connIdList.ForEach((otherConnId) =>
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
            }
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
                    connIdList.ForEach((connId) =>
                    {
                        rqs.SendUpdate_WRoleState(connId, roleEntity);
                    });
                }
            });
        }

        void Tick_MoveAndRotateOpt()
        {
            connIdList.ForEach((connId) =>
            {
                long key = (long)serveFrame << 32;
                key |= (long)connId;
                var roleRepo = battleFacades.ClientBattleFacades.Repo.RoleRepo;
                var rqs = battleFacades.Network.BattleRoleReqAndRes;

                if (roleMoveMsgDic.TryGetValue(key, out var moveMsg))
                {
                    var realMsg = moveMsg.msg;

                    var rid = (byte)(realMsg >> 48);
                    var role = roleRepo.GetByEntityId(rid);

                    // ------------移动
                    Vector3 dir = new Vector3((short)(realMsg >> 32) / 100f, (short)(realMsg >> 16) / 100f, (short)realMsg / 100f);

                    //服务器逻辑
                    role.MoveComponent.ActivateMoveVelocity(dir);

                    //发送状态同步帧
                    if (role.RoleState != RoleState.Hooking) role.SetRoleState(RoleState.Move);
                    connIdList.ForEach((otherConnId) =>
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
                    connIdList.ForEach((otherConnId) =>
                    {
                        rqs.SendUpdate_WRoleState(otherConnId, role);
                    });
                }

            });

        }

        int Tick_JumpOpt()
        {
            if (!jumpOptQueueDic.TryGetValue(serveFrame, out var optQueue)) return 0;

            int dequeueCount = 0;
            while (optQueue.TryDequeue(out var opt))
            {
                dequeueCount++;
                var wRid = opt.msg.wRid;
                var roleRepo = battleFacades.ClientBattleFacades.Repo.RoleRepo;
                var roleEntity = roleRepo.GetByEntityId(wRid);
                var rqs = battleFacades.Network.BattleRoleReqAndRes;

                //服务器逻辑Jump
                if (roleEntity.MoveComponent.TryJump())
                {
                    if (roleEntity.RoleState != RoleState.Hooking) roleEntity.SetRoleState(RoleState.Jump);
                    //发送状态同步帧
                    connIdList.ForEach((connId) =>
                    {
                        rqs.SendUpdate_WRoleState(connId, roleEntity);
                    });
                }
            }

            return dequeueCount;
        }
        #endregion

        #region [Bullet]
        void Tick_BulletSpawn()
        {
            if (bulletSpawnQueueDic.TryGetValue(serveFrame, out var spawnQueue))
            {

                while (spawnQueue.TryDequeue(out var spawn))
                {
                    int connId = spawn.connId;
                    var msg = spawn.msg;

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
                    Debug.Log($"服务器逻辑[生成子弹] serveFrame {serveFrame} connId {connId}:  bulletType:{bulletTypeByte.ToString()} bulletId:{bulletId}  MasterWRid:{wRid}  起点：{shootStartPoint} 终点：{targetPos} 飞行方向:{shootDir}");

                    var rqs = battleFacades.Network.BulletReqAndRes;
                    connIdList.ForEach((otherConnId) =>
                    {
                        rqs.SendRes_BulletSpawn(otherConnId, bulletType, bulletId, wRid, shootDir);
                    });
                }
            }
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
                connIdList.ForEach((connId) =>
                {
                    // 广播子弹销毁消息
                    bulletRqs.SendRes_BulletTearDown(connId, bulletType, bulletEntity.MasterId, bulletEntity.EntityId, bulletEntity.MoveComponent.CurPos);
                });
                while (effectRoleQueue.TryDequeue(out var role))
                {
                    Debug.Log($"角色击飞发送");
                    connIdList.ForEach((connId) =>
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
                    connIdList.ForEach((connId) =>
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
                connIdList.ForEach((connId) =>
                {
                    rqs.SendUpdate_WRoleState(connId, master);
                });
            });

        }
        #endregion

        #region [Item]
        void Tick_ItemPickUp()
        {
            if (itemPickUpQueueDic.TryGetValue(serveFrame, out var itemPickQueue))
            {
                while (itemPickQueue.TryPeek(out var msgStruct))
                {
                    itemPickQueue.Dequeue();

                    int connId = msgStruct.connId;
                    var msg = msgStruct.msg;
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
                        connIdList.ForEach((connId) =>
                        {
                            rqs.SendRes_ItemPickUp(connId, serveFrame, msg.wRid, itemType, msg.entityId);
                        });
                    }
                    else
                    {
                        Debug.Log($"{itemType.ToString()}物品拾取失败");
                    }
                }
            }
        }

        #endregion

        #region [Weapon]

        void Tick_WeaponShoot()
        {
            if (weaponShootQueueDic.TryGetValue(serveFrame, out var queue))
            {
                var clientFacades = battleFacades.ClientBattleFacades;
                var weaponRepo = clientFacades.Repo.WeaponRepo;
                var roleRepo = clientFacades.Repo.RoleRepo;
                var bulletRepo = clientFacades.Repo.BulletRepo;

                var weaponRqs = battleFacades.Network.WeaponReqAndRes;
                var bulletRqs = battleFacades.Network.BulletReqAndRes;

                var fieldEntity = clientFacades.Repo.FiledRepo.Get(1);

                while (queue.TryPeek(out var msgStruct))
                {
                    queue.Dequeue();
                    var msg = msgStruct.msg;
                    var masterId = msg.masterId;

                    if (roleRepo.TryGetByEntityId(masterId, out var master))
                    {
                        if (master.WeaponComponent.TryWeaponShoot())
                        {
                            //子弹生成
                            float targetPosX = msg.targetPosX / 10000f;
                            float targetPosY = msg.targetPosY / 10000f;
                            float targetPosZ = msg.targetPosZ / 10000f;
                            Vector3 targetPos = new Vector3(targetPosX, targetPosY, targetPosZ);
                            var shootStartPoint = master.ShootPointPos;
                            Vector3 shootDir = targetPos - shootStartPoint;
                            shootDir.Normalize();

                            var bulletType = master.WeaponComponent.CurrentWeapon.bulletType;
                            var bulletEntity = clientFacades.Domain.BulletDomain.SpawnBullet(fieldEntity.transform, bulletType);
                            var bulletId = bulletRepo.BulletCount;
                            bulletEntity.MoveComponent.SetCurPos(shootStartPoint);
                            bulletEntity.MoveComponent.SetForward(shootDir);
                            bulletEntity.MoveComponent.ActivateMoveVelocity(shootDir);
                            bulletEntity.SetMasterId(masterId);
                            bulletEntity.SetEntityId(bulletId);
                            bulletEntity.gameObject.SetActive(true);
                            bulletRepo.Add(bulletEntity);

                            connIdList.ForEach((connId) =>
                            {
                                weaponRqs.SendRes_WeaponShoot(connId, masterId);
                                bulletRqs.SendRes_BulletSpawn(connId, bulletType, bulletId, masterId, shootDir);
                            });
                            Debug.Log($"生成子弹bulletType:{bulletType.ToString()} bulletId:{bulletId}  MasterWRid:{masterId}  起点：{shootStartPoint} 终点：{targetPos} 飞行方向:{shootDir}");
                        }
                    }
                }
            }
        }

        void Tick_WeaponReload()
        {
            if (weaponReloadQueueDic.TryGetValue(serveFrame, out var queue))
            {
                var weaponRepo = battleFacades.ClientBattleFacades.Repo.WeaponRepo;
                var roleRepo = battleFacades.ClientBattleFacades.Repo.RoleRepo;
                var rqs = battleFacades.Network.WeaponReqAndRes;
                while (queue.TryPeek(out var msgStruct))
                {
                    queue.Dequeue();
                    var msg = msgStruct.msg;
                    var masterId = msg.masterId;
                    if (roleRepo.TryGetByEntityId(masterId, out var master))
                    {
                        if (master.TryWeaponReload(out int reloadBulletNum))
                        {
                            //TODO: 装弹时间过后才发送回客户端
                            connIdList.ForEach((connId) =>
                            {
                                rqs.SendRes_WeaponReloaded(connId, serveFrame, masterId, reloadBulletNum);
                            });
                        }
                    }
                }

            }
        }

        void Tick_WeaponDrop()
        {
            if (weaponDropQueueDic.TryGetValue(serveFrame, out var queue))
            {
                var weaponRepo = battleFacades.ClientBattleFacades.Repo.WeaponRepo;
                var roleRepo = battleFacades.ClientBattleFacades.Repo.RoleRepo;
                var rqs = battleFacades.Network.WeaponReqAndRes;
                while (queue.TryPeek(out var msgStruct))
                {
                    queue.Dequeue();
                    var msg = msgStruct.msg;
                    var entityId = msg.entityId;
                    var masterId = msg.masterId;
                    if (roleRepo.TryGetByEntityId(masterId, out var master)
                    && master.WeaponComponent.TryDropWeapon(entityId, out var weapon))
                    {
                        // 服务器逻辑
                        battleFacades.ClientBattleFacades.Domain.WeaponDomain.ReuseWeapon(weapon, master.MoveComponent.CurPos);

                        connIdList.ForEach((connId) =>
                        {
                            rqs.SendRes_WeaponDrop(connId, serveFrame, masterId, entityId);
                        });
                    }
                }
            }
        }

        #endregion

        #endregion

        // ====== Network
        // Role
        void OnRoleMove(int connId, FrameRoleMoveReqMsg msg)
        {
            lock (roleMoveMsgDic)
            {
                long key = (long)serveFrame << 32;
                key |= (long)connId;
                if (!roleMoveMsgDic.TryGetValue(key, out var opt))
                {
                    roleMoveMsgDic[key] = msg;
                }
            }
        }
        void OnRoleRotate(int connId, FrameRoleRotateReqMsg msg)
        {
            lock (roleMoveMsgDic)
            {
                long key = (long)serveFrame << 32;
                key |= (long)connId;
                if (!roleRotateMsgDic.TryGetValue(key, out var opt))
                {
                    roleRotateMsgDic[key] = msg;
                }
            }
        }

        void OnRoleJump(int connId, FrameJumpReqMsg msg)
        {
            lock (jumpOptQueueDic)
            {
                if (!jumpOptQueueDic.TryGetValue(serveFrame, out var jumpOptList))
                {
                    jumpOptList = new Queue<FrameJumpReqMsgStruct>();
                    jumpOptQueueDic[serveFrame] = jumpOptList;
                }

                jumpOptList.Enqueue(new FrameJumpReqMsgStruct { connId = connId, msg = msg });
            }
        }

        void OnBattleRoleSpawn(int connId, FrameBattleRoleSpawnReqMsg msg)
        {
            lock (wRoleSpawQueuenDic)
            {
                Debug.Log($"[战斗Controller] 战斗角色生成请求");
                if (!wRoleSpawQueuenDic.TryGetValue(serveFrame, out var queue))
                {
                    queue = new Queue<FrameWRoleSpawnReqMsgStruct>();
                    wRoleSpawQueuenDic[serveFrame] = queue;
                }

                queue.Enqueue(new FrameWRoleSpawnReqMsgStruct { connId = connId, msg = msg });
            }

            // TODO:连接服和世界服分离
            connIdList.Add(connId);
            // 创建场景(First Time)
            sceneSpawnTrigger = true;
        }

        void OnBulletSpawn(int connId, FrameBulletSpawnReqMsg msg)
        {
            lock (bulletSpawnQueueDic)
            {
                if (!bulletSpawnQueueDic.TryGetValue(serveFrame, out var queue))
                {
                    queue = new Queue<FrameBulletSpawnReqMsgStruct>();
                    bulletSpawnQueueDic[serveFrame] = queue;
                }

                queue.Enqueue(new FrameBulletSpawnReqMsgStruct { connId = connId, msg = msg });
            }
        }

        // ========= Item
        void OnItemPickUp(int connId, FrameItemPickReqMsg msg)
        {
            lock (itemPickUpQueueDic)
            {
                if (!itemPickUpQueueDic.TryGetValue(serveFrame, out var msgStruct))
                {
                    msgStruct = new Queue<FrameItemPickUpReqMsgStruct>();
                    itemPickUpQueueDic[serveFrame] = msgStruct;
                }

                msgStruct.Enqueue(new FrameItemPickUpReqMsgStruct { connId = connId, msg = msg });
            }
        }

        // =========== Weapon
        void OnWeaponShoot(int connId, FrameWeaponShootReqMsg msg)
        {
            lock (weaponShootQueueDic)
            {
                if (!weaponShootQueueDic.TryGetValue(serveFrame, out var msgStruct))
                {
                    msgStruct = new Queue<FrameWeaponShootReqMsgStruct>();
                    weaponShootQueueDic[serveFrame] = msgStruct;
                }

                msgStruct.Enqueue(new FrameWeaponShootReqMsgStruct { connId = connId, msg = msg });
                Debug.Log("收到武器射击请求");
            }
        }

        void OnWeaponReload(int connId, FrameWeaponReloadReqMsg msg)
        {
            lock (weaponReloadQueueDic)
            {
                if (!weaponReloadQueueDic.TryGetValue(serveFrame, out var msgStruct))
                {
                    msgStruct = new Queue<FrameWeaponReloadReqMsgStruct>();
                    weaponReloadQueueDic[serveFrame] = msgStruct;
                }

                msgStruct.Enqueue(new FrameWeaponReloadReqMsgStruct { connId = connId, msg = msg });
                Debug.Log("收到武器换弹请求");
            }
        }

        void OnWeaponDrop(int connId, FrameWeaponDropReqMsg msg)
        {
            lock (weaponDropQueueDic)
            {
                if (!weaponDropQueueDic.TryGetValue(serveFrame, out var msgStruct))
                {
                    msgStruct = new Queue<FrameWeaponDropReqMsgStruct>();
                    weaponDropQueueDic[serveFrame] = msgStruct;
                }

                msgStruct.Enqueue(new FrameWeaponDropReqMsgStruct { connId = connId, msg = msg });
                Debug.Log("收到武器丢弃请求");
            }
        }

        // ====== Scene Spawn Method
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

    }

}
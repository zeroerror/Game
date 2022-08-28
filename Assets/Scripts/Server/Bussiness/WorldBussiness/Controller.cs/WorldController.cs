using System.Collections.Generic;
using UnityEngine;
using Game.Server.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;
using Game.Client.Bussiness.WorldBussiness;

namespace Game.Server.Bussiness.WorldBussiness
{

    public class WorldController
    {

        WorldFacades worldFacades;
        int worldServeFrameIndex;

        // 记录当前所有ConnId
        List<int> connIdList;

        // 记录所有操作帧
        struct FrameReqOptMsgStruct
        {
            public int connId;
            public FrameOptReqMsg msg;
        }
        Dictionary<int, FrameReqOptMsgStruct> wRoleOptDic;

        // 移动记录所有跳跃帧
        struct FrameReqJumpMsgStruct
        {
            public int connId;
            public FrameJumpReqMsg msg;
        }
        Dictionary<int, FrameReqJumpMsgStruct> jumpOptDic;

        // 记录所有生成帧
        struct FrameReqWRoleSpawnMsgStruct
        {
            public int connId;
            public FrameWRoleSpawnReqMsg msg;
        }
        Dictionary<int, FrameReqWRoleSpawnMsgStruct> wRoleSpawnDic;

        // 记录所有子弹生成帧
        struct FrameReqBulletSpawnMsgStruct
        {
            public int connId;
            public FrameBulletSpawnReqMsg msg;
        }
        Dictionary<int, FrameReqBulletSpawnMsgStruct> bulletSpawnDic;

        bool sceneSpawnTrigger;
        bool isSceneSpawn;

        public WorldController()
        {
            connIdList = new List<int>();
            wRoleOptDic = new Dictionary<int, FrameReqOptMsgStruct>();
            jumpOptDic = new Dictionary<int, FrameReqJumpMsgStruct>();
            wRoleSpawnDic = new Dictionary<int, FrameReqWRoleSpawnMsgStruct>();
            bulletSpawnDic = new Dictionary<int, FrameReqBulletSpawnMsgStruct>();
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;

            var roleRqs = worldFacades.Network.WorldRoleReqAndRes;
            roleRqs.RegistReq_WorldRoleMove(OnWoldRoleMove);
            roleRqs.RegistReq_Jump(OnWoldRoleJump);
            roleRqs.RegistReq_WolrdRoleSpawn(OnWoldRoleSpawn);

            var bulletRqs = worldFacades.Network.BulletReqAndRes;
            bulletRqs.RegistReq_BulletSpawn(OnBulletSpawn);
        }

        public void Tick()
        {
            // Tick的过滤条件
            if (sceneSpawnTrigger && !isSceneSpawn)
            {
                SpawWorldChooseScene();
                sceneSpawnTrigger = false;
            }
            if (!isSceneSpawn) return;

            Tick_WRoleSpawn();
            Tick_BulletSpawn();
            Tick_JumpOpt();
            Tick_Opt();
            Tick_RoleStateSync();
        }

        // 目前流程是会先生成唯一的控制角色，所以这里相当于入口，在这里判断是否是中途加入，是否需要补发数据包
        void Tick_WRoleSpawn()
        {
            int nextFrameIndex = worldServeFrameIndex + 1;
            if (wRoleSpawnDic.TryGetValue(nextFrameIndex, out var spawn))
            {
                worldServeFrameIndex = nextFrameIndex;

                var msg = spawn.msg;
                var connId = spawn.connId;
                var clientFrameIndex = msg.clientFrameIndex;

                var clientFacades = worldFacades.ClientWorldFacades;
                var repo = clientFacades.Repo;
                var fieldEntity = repo.FiledEntityRepo.Get(1);
                var rqs = worldFacades.Network.WorldRoleReqAndRes;
                var roleRepo = repo.WorldRoleRepo;
                var wrid = roleRepo.Size;

                // 服务器逻辑
                var roleEntity = clientFacades.Domain.WorldRoleSpawnDomain.SpawnWorldRole(fieldEntity.transform);
                roleEntity.SetWRid(wrid);
                Debug.Log($"服务器逻辑[Spawn Role] frame:{worldServeFrameIndex} wRid:{wrid}  roleEntity.MoveComponent.CurPos:{roleEntity.MoveComponent.CurPos}");

                if (clientFrameIndex + 1 < worldServeFrameIndex)
                {

                    // ====== 补发数据包
                    // Debug.Log("所有人物操作包数据========================================");
                    // for (int frameIndex = clientFrameIndex + 1; frameIndex < worldServeFrameIndex; frameIndex++)
                    // {
                    //     if (!optDic.TryGetValue(frameIndex, out var optMsgStruct)) continue;

                    //     rqs.ResendRes_WorldRoleMove(connId, frameIndex, optMsgStruct.msg);
                    // }

                    // ====== 发送其他角色的状态同步帧给请求者
                    var allEntity = roleRepo.GetAll();
                    for (int i = 0; i < allEntity.Length; i++)
                    {
                        var otherRole = allEntity[i];
                        rqs.SendUpdate_WRoleState(connId, nextFrameIndex, otherRole.WRid, otherRole.RoleStatus, otherRole.transform.position, otherRole.transform.rotation, otherRole.MoveComponent.Velocity);
                    }

                    // ====== 广播请求者创建的角色给其他人
                    connIdList.ForEach((otherConnId) =>
                    {
                        if (otherConnId != connId)
                        {
                            rqs.SendUpdate_WRoleState(otherConnId, nextFrameIndex, wrid, roleEntity.RoleStatus, roleEntity.transform.position, roleEntity.transform.rotation, roleEntity.MoveComponent.Velocity);
                        }
                    });

                    // ====== 回复请求者创建的角色
                    rqs.SendUpdate_WRoleState(connId, nextFrameIndex, wrid, roleEntity.RoleStatus, roleEntity.transform.position, roleEntity.transform.rotation, roleEntity.MoveComponent.Velocity, true);

                }
                else
                {
                    Debug.Log($"服务端回复消息[生成帧] {nextFrameIndex}--------------------------------------------------------------------------");
                    rqs.SendRes_WorldRoleSpawn(connId, nextFrameIndex, wrid, true);
                }

                roleRepo.Add(roleEntity);
            }
        }

        void Tick_Opt()
        {
            int nextFrameIndex = worldServeFrameIndex + 1;
            if (wRoleOptDic.TryGetValue(nextFrameIndex, out var opt))
            {
                worldServeFrameIndex = nextFrameIndex;

                var msg = opt.msg;
                var realMsg = msg.msg;
                var connId = opt.connId;

                var rid = (byte)(realMsg >> 24);
                var roleRepo = worldFacades.ClientWorldFacades.Repo.WorldRoleRepo;
                var roleEntity = roleRepo.Get(rid);
                var optTypeId = opt.msg.optTypeId;
                var rqs = worldFacades.Network.WorldRoleReqAndRes;
                if (optTypeId == 1)
                {
                    Vector3 dir = new Vector3((sbyte)(realMsg >> 16), (sbyte)(realMsg >> 8), (sbyte)realMsg);
                    //服务器逻辑Move
                    roleEntity.MoveComponent.Move(dir);
                    roleEntity.MoveComponent.FaceTo(dir);

                    // 刷新人物状态
                    UpdateWRoleStatus(roleEntity);

                    //发送状态同步帧
                    connIdList.ForEach((connId) =>
                    {
                        rqs.SendUpdate_WRoleState(connId, nextFrameIndex, rid, RoleState.Move, roleEntity.transform.position, roleEntity.transform.rotation, roleEntity.MoveComponent.Velocity);
                    });
                }
            }
        }

        void Tick_JumpOpt()
        {
            int nextFrameIndex = worldServeFrameIndex + 1;
            if (jumpOptDic.TryGetValue(nextFrameIndex, out var jumpOpt))
            {
                worldServeFrameIndex = nextFrameIndex;

                var wRid = jumpOpt.msg.wRid;
                var roleRepo = worldFacades.ClientWorldFacades.Repo.WorldRoleRepo;
                var roleEntity = roleRepo.Get(wRid);
                var rqs = worldFacades.Network.WorldRoleReqAndRes;

                //服务器逻辑Jump
                roleEntity.MoveComponent.Jump();
                UpdateWRoleStatus(roleEntity);

                //发送状态同步帧
                connIdList.ForEach((connId) =>
                {
                    rqs.SendUpdate_WRoleState(connId, nextFrameIndex, wRid, RoleState.Jump, roleEntity.transform.position, roleEntity.transform.rotation, roleEntity.MoveComponent.Velocity);
                });
            }
        }

        void Tick_BulletSpawn()
        {
            int nextFrameIndex = worldServeFrameIndex + 1;
            if (bulletSpawnDic.TryGetValue(nextFrameIndex, out var bulletSpawn))
            {
                worldServeFrameIndex = nextFrameIndex;

                int connId = bulletSpawn.connId;
                var msg = bulletSpawn.msg;

                byte wRid = msg.wRid;
                float targetPosX = msg.targetPosX / 10000f;
                float targetPosY = msg.targetPosY / 10000f;
                float targetPosZ = msg.targetPosZ / 10000f;
                Vector3 targetPos = new Vector3(targetPosX, targetPosY, targetPosZ);
                targetPos.y = 0;
                var roleEntity = worldFacades.ClientWorldFacades.Repo.WorldRoleRepo.Get(msg.wRid);
                var moveComponent = roleEntity.MoveComponent;
                var shootStartPoint = roleEntity.ShootPointPos;
                Vector3 dir = targetPos - shootStartPoint;
                dir.Normalize();

                // 服务器逻辑
                var clientFacades = worldFacades.ClientWorldFacades;
                var fieldEntity = clientFacades.Repo.FiledEntityRepo.Get(1);
                var bulletEntity = clientFacades.Domain.BulletSpawnDomain.SpawnBullet(fieldEntity.transform);
                var bulletRepo = clientFacades.Repo.BulletEntityRepo;
                var bulletId = bulletRepo.Size;
                bulletEntity.MoveComponent.SetCurPos(shootStartPoint);
                bulletEntity.MoveComponent.Move(dir);
                bulletEntity.SetWRid(wRid);
                bulletEntity.SetBulletId(bulletId);
                bulletRepo.Add(bulletEntity);
                Debug.Log($"服务器逻辑[Spawn Bullet] frame {worldServeFrameIndex} connId {connId}    :MasterWRid:{wRid}  bulletId:{bulletId} 起点：{shootStartPoint} 终点：{targetPos} 飞行方向:{dir}");

                var rqs = worldFacades.Network.BulletReqAndRes;
                connIdList.ForEach((otherConnId) =>
                {
                    rqs.SendRes_BulletSpawn(otherConnId, worldServeFrameIndex, bulletId, wRid, dir);
                });
            }
        }

        void Tick_RoleStateSync()
        {
            int nextFrameIndex = worldServeFrameIndex + 1;
            //人物静止和运动 2个状态
            bool isNextFrame = false;
            var WorldRoleRepo = worldFacades.ClientWorldFacades.Repo.WorldRoleRepo;
            WorldRoleRepo.Foreach((roleEntity) =>
            {
                if (roleEntity.IsStateChange(out RoleState roleNewStatus))
                {
                    isNextFrame = true;
                    roleEntity.SetRoleStatus(roleNewStatus);


                    if (roleNewStatus == RoleState.Idle)
                    {
                        // 从运动到静止：因为运动的这个过程没有实时记录角色的状态位置信息LastSyncFramePos,所有这种情况下需要手动刷新下
                        roleEntity.MoveComponent.UpdateLastSyncFramePos();
                    }
                    var rqs = worldFacades.Network.WorldRoleReqAndRes;
                    connIdList.ForEach((connId) =>
                    {
                        rqs.SendUpdate_WRoleState(connId, nextFrameIndex, roleEntity.WRid, roleNewStatus, roleEntity.MoveComponent.LastSyncFramePos, roleEntity.transform.rotation, roleEntity.MoveComponent.Velocity);
                        roleEntity.MoveComponent.UpdateLastSyncFramePos();
                    });
                }
            });

            if (isNextFrame)
            {
                worldServeFrameIndex = nextFrameIndex;
            }
        }

        void UpdateWRoleStatus(WorldRoleEntity roleEntity)
        {
            if (roleEntity.IsStateChange(out RoleState newRoleState))
            {
                roleEntity.SetRoleStatus(newRoleState);
            }
        }


        // == Network
        // Role
        void OnWoldRoleMove(int connId, FrameOptReqMsg msg)
        {
            wRoleOptDic.TryAdd(worldServeFrameIndex + 1, new FrameReqOptMsgStruct { connId = connId, msg = msg });
        }

        void OnWoldRoleJump(int connId, FrameJumpReqMsg msg)
        {
            jumpOptDic.TryAdd(worldServeFrameIndex + 1, new FrameReqJumpMsgStruct { connId = connId, msg = msg });
        }

        void OnWoldRoleSpawn(int connId, FrameWRoleSpawnReqMsg msg)
        {
            wRoleSpawnDic.TryAdd(worldServeFrameIndex + 1, new FrameReqWRoleSpawnMsgStruct { connId = connId, msg = msg });
            // TODO:连接服和世界服分离
            connIdList.Add(connId);
            // 创建场景
            sceneSpawnTrigger = true;
        }

        // Bullet
        void OnBulletSpawn(int connId, FrameBulletSpawnReqMsg msg)
        {
            bulletSpawnDic.TryAdd(worldServeFrameIndex + 1, new FrameReqBulletSpawnMsgStruct { connId = connId, msg = msg });
        }

        async void SpawWorldChooseScene()
        {
            // Load Scene And Spawn Field
            var domain = worldFacades.ClientWorldFacades.Domain;
            var fieldEntity = await domain.WorldSpawnDomain.SpawnWorldChooseScene();
            fieldEntity.SetFieldId(1);
            var fieldEntityRepo = worldFacades.ClientWorldFacades.Repo.FiledEntityRepo;
            fieldEntityRepo.Add(fieldEntity);
            isSceneSpawn = true;
        }

    }

}
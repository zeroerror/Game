using System.Collections.Generic;
using UnityEngine;
using Game.Server.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;
using UnityEngine.SceneManagement;
using Game.Client.Bussiness.WorldBussiness;

namespace Game.Server.Bussiness.WorldBussiness
{

    public class WorldController
    {

        WorldFacades worldFacades;
        int worldServeFrameIndex;
        byte wRid;

        // 记录当前所有ConnId
        List<int> connIdList;

        // 记录所有操作帧
        struct FrameReqOptMsgStruct
        {
            public int connId;
            public FrameOptReqMsg msg;
        }
        Dictionary<int, FrameReqOptMsgStruct> optDic;

        // 记录所有生成帧
        struct FrameReqWRoleSpawnMsgStruct
        {
            public int connId;
            public FrameWRoleSpawnReqMsg msg;
        }
        Dictionary<int, FrameReqWRoleSpawnMsgStruct> spawnDic;

        bool sceneSpawnTrigger;
        bool isSceneSpawn;

        public WorldController()
        {
            optDic = new Dictionary<int, FrameReqOptMsgStruct>();
            spawnDic = new Dictionary<int, FrameReqWRoleSpawnMsgStruct>();
            connIdList = new List<int>();
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;

            var rqs = worldFacades.Network.WorldRoleReqAndRes;
            rqs.RegistReq_WorldRoleMove(OnWoldRoleMove);
            rqs.RegistReq_WolrdRoleSpawn(OnWoldRoleSpawn);
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

            // 每一帧判断人物状态是否改变，是否需要发送客户端更新人物状态信息
            Tick_RoleStateSync();
            // 对客户端发送来的生成帧和操作帧进行处理
            Tick_WRoleSpawn();
            Tick_Opt();
        }

        // 目前流程是会先生成唯一的控制角色，所以这里相当于入口，在这里判断是否是中途加入，是否需要补发数据包
        private void Tick_WRoleSpawn()
        {

            int nextFrameIndex = worldServeFrameIndex + 1;
            if (spawnDic.TryGetValue(nextFrameIndex, out var spawn))
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
                wRid++;

                // 服务器逻辑
                var roleEntity = clientFacades.Domain.WorldRoleSpawnDomain.SpawnWorldRole(fieldEntity.transform);
                roleEntity.SetWRid(wRid);
                Debug.Log($"服务器逻辑[Spawn] frame:{worldServeFrameIndex} wRid:{wRid}");

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
                        rqs.SendUpdate_WRoleState(connId, nextFrameIndex, otherRole.WRid, otherRole.RoleStatus, otherRole.transform.position);
                    }

                    // ====== 广播请求者创建的角色给其他人
                    connIdList.ForEach((otherConnId) =>
                    {
                        if (otherConnId != connId)
                        {
                            rqs.SendUpdate_WRoleState(otherConnId, nextFrameIndex, wRid, roleEntity.RoleStatus, roleEntity.transform.position);
                        }
                    });

                    // ====== 回复请求者创建的角色
                    rqs.SendUpdate_WRoleState(connId, nextFrameIndex, wRid, roleEntity.RoleStatus, roleEntity.transform.position, true);

                }
                else
                {
                    Debug.Log($"服务端回复消息[生成帧] {nextFrameIndex}--------------------------------------------------------------------------");
                    rqs.SendRes_WorldRoleSpawn(connId, nextFrameIndex, wRid, true);
                }

                roleRepo.Add(roleEntity);
            }
        }

        void Tick_Opt()
        {
            int nextFrameIndex = worldServeFrameIndex + 1;
            if (optDic.TryGetValue(nextFrameIndex, out var opt))
            {
                worldServeFrameIndex = nextFrameIndex;

                var msg = opt.msg;
                var realMsg = msg.msg;
                var connId = opt.connId;

                var rid = (byte)(realMsg >> 24);
                Vector3 dir = new Vector3((sbyte)(realMsg >> 16), (sbyte)(realMsg >> 8), (sbyte)realMsg);

                var roleRepo = worldFacades.ClientWorldFacades.Repo.WorldRoleRepo;
                var roleEntity = roleRepo.Get(rid);
                var optTypeId = opt.msg.optTypeId;
                if (optTypeId == 1)
                {
                    //服务器逻辑移动
                    roleEntity.MoveComponent.Move(dir);

                    // 刷新人物状态
                    bool isMove = roleEntity.MoveComponent.Velocity != Vector3.zero;
                    bool isMoveStatus = roleEntity.RoleStatus != RoleState.Idle;
                    if (roleEntity.IsStateChange(out RoleState newRoleState))
                    {
                        roleEntity.SetRoleStatus(newRoleState);
                    }

                    //发送状态同步帧
                    var rqs = worldFacades.Network.WorldRoleReqAndRes;
                    connIdList.ForEach((connId) =>
                    {
                        rqs.SendUpdate_WRoleState(connId, nextFrameIndex, rid, RoleState.Move, roleEntity.transform.position);
                    });

                }

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

                    var rqs = worldFacades.Network.WorldRoleReqAndRes;
                    connIdList.ForEach((connId) =>
                    {
                        rqs.SendUpdate_WRoleState(connId, nextFrameIndex, roleEntity.WRid, roleNewStatus, roleEntity.transform.position);
                    });
                }
            });
            if (isNextFrame) worldServeFrameIndex = nextFrameIndex;
        }

        // Network
        void OnWoldRoleMove(int connId, FrameOptReqMsg msg)
        {
            optDic.TryAdd(worldServeFrameIndex + 1, new FrameReqOptMsgStruct { connId = connId, msg = msg });

        }

        void OnWoldRoleSpawn(int connId, FrameWRoleSpawnReqMsg msg)
        {
            int nextFrameIndex = worldServeFrameIndex + 1;
            spawnDic.TryAdd(nextFrameIndex, new FrameReqWRoleSpawnMsgStruct { connId = connId, msg = msg });
            // TODO:连接服和世界服分离
            connIdList.Add(connId);
            // 创建场景
            sceneSpawnTrigger = true;
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
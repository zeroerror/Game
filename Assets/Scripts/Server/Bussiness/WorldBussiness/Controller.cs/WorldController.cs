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
        byte wRoleId;

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

                if (clientFrameIndex + 1 < worldServeFrameIndex)
                {

                    // ====== 补发数据包
                    Debug.Log($"中途加入，补发数据包：connId:{connId} clientFrameIndex:{clientFrameIndex} 到 worldServeFrameIndex:{worldServeFrameIndex}");

                    Debug.Log("所有人物生成包数据========================================");
                    var allEntity = roleRepo.GetAll();
                    int index = 0;
                    for (int frameIndex = clientFrameIndex + 1; frameIndex < worldServeFrameIndex; frameIndex++)
                    {
                        if (!spawnDic.TryGetValue(frameIndex, out var spawnMsgStruct)) continue;

                        rqs.ResendRes_WorldRoleSpawn(connId, frameIndex, allEntity[index++].WRid, false);
                    }
                    // 加上当前帧
                    rqs.ResendRes_WorldRoleSpawn(connId, worldServeFrameIndex, ++wRoleId, true);

                    Debug.Log("所有人物操作包数据========================================");
                    for (int frameIndex = clientFrameIndex + 1; frameIndex < worldServeFrameIndex; frameIndex++)
                    {
                        if (!optDic.TryGetValue(frameIndex, out var optMsgStruct)) continue;

                        rqs.ResendRes_WorldRoleMove(connId, frameIndex, optMsgStruct.msg);
                    }

                    // ====== 广播给其他人
                    connIdList.ForEach((otherConnId) =>
                    {
                        if (otherConnId != connId)
                        {
                            rqs.SendRes_WorldRoleSpawn(otherConnId, worldServeFrameIndex, wRoleId, false);
                        }
                    });
                }
                else
                {
                    Debug.Log($"服务端回复消息[生成帧] {worldServeFrameIndex}--------------------------------------------------------------------------");
                    rqs.SendRes_WorldRoleSpawn(connId, worldServeFrameIndex, ++wRoleId, true);
                }

                // 服务器逻辑
                var entity = clientFacades.Domain.WorldRoleSpawnDomain.SpawnWorldRole(fieldEntity.transform);
                entity.SetWRid(wRoleId);
                roleRepo.Add(entity);
                Debug.Log($"服务器逻辑[Spawn] frame:{worldServeFrameIndex}");
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
                    roleEntity.MoveComponent.Move(dir); //服务器逻辑移动

                    // 根据状态是否改变判断是否回复给客户端
                    bool isMove = roleEntity.MoveComponent.Velocity != Vector3.zero;
                    bool isMoveStatus = roleEntity.RoleStatus != RoleState.Stand;
                    if ((isMove && !isMoveStatus) || (!isMove && isMoveStatus))
                    {
                        // 刷新人物状态
                        var roleNewStatus = isMove ? RoleState.Move : RoleState.Stand;
                        roleEntity.SetRoleStatus(roleNewStatus);

                        //发送状态同步帧
                        var rqs = worldFacades.Network.WorldRoleReqAndRes;
                        //状态帧同步就不需要发送确认操作帧了
                        // Debug.Log($"服务端广播回复消息[操作帧]--------------------------------------------------------------------------");
                        // connIdList.ForEach((connId) =>
                        // {
                        //     rqs.SendRes_WorldRoleMove(connId, worldServeFrameIndex, msg);
                        //     Debug.Log($"SendFrame: {worldServeFrameIndex} [connId:{connId}] 确认人物移动  rid:{rid}  dir:{dir}");
                        // });

                        connIdList.ForEach((connId) =>
                      {
                          rqs.SendUpdate_WRoleState(connId, nextFrameIndex, rid, RoleState.Move, roleEntity.transform.position);
                      });
                    }

                }



                // 服务器逻辑


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
                bool isMove = roleEntity.MoveComponent.Velocity != Vector3.zero;
                bool isMoveStatus = roleEntity.RoleStatus != RoleState.Stand;
                if ((isMove && !isMoveStatus) || (!isMove && isMoveStatus))
                {
                    isNextFrame = true;
                    var roleNewStatus = isMove ? RoleState.Move : RoleState.Stand;
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
using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {
        WorldFacades worldFacades;
        int worldClientFrameIndex;

        // 操作队列
        Queue<FrameOptResMsg> optQueue;
        List<FrameOptResResendMsg> resendList_Opt;

        // 生成队列
        Queue<FrameWRoleSpawnResMsg> spawnQueue;
        List<FrameWRoleSpawnResResendMsg> resendList_Spawn;


        public WorldController()
        {
            // Between Bussiness
            NetworkEventCenter.RegistLoginSuccess(EnterWorldChooseScene);

            optQueue = new Queue<FrameOptResMsg>();
            resendList_Opt = new List<FrameOptResResendMsg>();

            spawnQueue = new Queue<FrameWRoleSpawnResMsg>();
            resendList_Spawn = new List<FrameWRoleSpawnResResendMsg>();
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
            var req = worldFacades.Network.WorldRoleReqAndRes;
            req.RegistRes_WorldRoleOpt(OnWorldRoleOpt);
            req.RegistRes_WorldRoleSpawn(OnWorldRoleSpawn);
            req.RegistResResend_WorldRoleSpawn(OnWorldRoleSpawnResend);
            req.RegistResResend_Opt(OnWorldRoleOptResend);
        }

        public void Tick()
        {
            Tick_Input();
            Tick_ServerRes();
        }

        void Tick_ServerRes()
        {
            if (optQueue.TryPeek(out var opt) && worldClientFrameIndex + 1 == opt.serverFrameIndex)
            {
                optQueue.Dequeue();
                worldClientFrameIndex++;
                Debug.Log($"操作帧 FrameIndex:{worldClientFrameIndex}");

                var optTypeId = opt.optTypeId;
                // 解析操作
                if (optTypeId == 1)
                {
                    //移动操作
                    var realMsg = opt.msg;
                    var rid = (byte)(realMsg >> 24);
                    Vector3 dir = new Vector3((sbyte)(realMsg >> 16), (sbyte)(realMsg >> 8), (sbyte)realMsg);
                    float fixedSpeed = 1f;
                    Vector3 velocity = fixedSpeed * dir;
                    var roleEntity = worldFacades.Repo.WorldRoleRepo.Get(rid);

                    roleEntity.Move(velocity);
                    Debug.Log($"移动  WRid:{rid}  velocity: {velocity}");
                }
            }

            if (spawnQueue.TryPeek(out var spawn) && worldClientFrameIndex + 1 == spawn.serverFrameIndex)
            {
                spawnQueue.Dequeue();
                worldClientFrameIndex++;
                Debug.Log($"生成帧 FrameIndex:{worldClientFrameIndex}");
                var wRoleId = spawn.wRoleId;
                var repo = worldFacades.Repo;
                var fieldEntity = repo.FiledEntityRepo.Get(1);
                var domain = worldFacades.Domain.WorldRoleSpawnDomain;
                var entity = domain.SpawnWorldRole(fieldEntity.transform);
                entity.SetRid(wRoleId);

                var roleRepo = repo.WorldRoleRepo;
                roleRepo.Add(entity);
                if (spawn.isOwner)
                {
                    roleRepo.SetOwner(entity);
                    worldFacades.CinemachineExtra.FollowSolo(entity.transform, 3f);
                    worldFacades.CinemachineExtra.LookAtSolo(entity.CamTrackingObj, 3f);
                }

                Debug.Log(spawn.isOwner ? $"生成自身角色 : WRid:{entity.WRid}" : $"生成其他角色 : WRid:{entity.WRid}");
            }

        }

        void Tick_Input()
        {
            //没有角色就没有移动
            var owner = worldFacades.Repo.WorldRoleRepo.Owner;
            if (owner == null) return;

            bool needMove = false;
            Vector3 moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                needMove = true;
                moveDir += new Vector3(0, 0, 1);
            }
            if (Input.GetKey(KeyCode.S))
            {
                needMove = true;
                moveDir += new Vector3(0, 0, -1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                needMove = true;
                moveDir += new Vector3(-1, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                needMove = true;
                moveDir += new Vector3(1, 0, 0);
            }

            if (needMove)
            {
                byte rid = worldFacades.Repo.WorldRoleRepo.Owner.WRid;
                worldFacades.Network.WorldRoleReqAndRes.SendReq_WorldRoleMove(worldClientFrameIndex, rid, moveDir);
            }
        }

        // == Server Response ==
        // OPT & SPAWN
        void OnWorldRoleOpt(FrameOptResMsg msg)
        {
            Debug.Log("加入操作队列");
            // 加入操作队列(TODO: optTypeId msg 合成一个ulong，从高位开始读起)
            optQueue.Enqueue(msg);
        }

        void OnWorldRoleSpawn(FrameWRoleSpawnResMsg msg)
        {
            Debug.Log("加入生成队列");
            // 加入生成队列
            spawnQueue.Enqueue(msg);
        }

        // RESEND
        void OnWorldRoleOptResend(FrameOptResResendMsg msg)
        {
            resendList_Opt.Add(msg);
            // TODO: 排序 验证完整性
            FrameOptResMsg enqueueMsg = new FrameOptResMsg
            {
                serverFrameIndex = msg.serverFrameIndex,
                optTypeId = msg.optTypeId,
                msg = msg.msg
            };
            optQueue.Enqueue(enqueueMsg);
        }

        void OnWorldRoleSpawnResend(FrameWRoleSpawnResResendMsg msg)
        {
            resendList_Spawn.Add(msg);
            // TODO: 排序 验证完整性
            // resendList_Spawn.Sort((a, b) =>
            // {
            //     if (a.serverFrameIndex > b.serverFrameIndex) return 1;
            //     else if (a.serverFrameIndex == b.serverFrameIndex) return 0;
            //     else return -1;
            // });
            FrameWRoleSpawnResMsg enqueueMsg = new FrameWRoleSpawnResMsg
            {
                serverFrameIndex = msg.serverFrameIndex,
                wRoleId = msg.wRoleId,
                isOwner = msg.isOwner
            };
            spawnQueue.Enqueue(enqueueMsg);
        }

        // NetworkEventCenter
        async void EnterWorldChooseScene()
        {
            // Load Scene And Spawn Field
            var domain = worldFacades.Domain;
            var fieldEntity = await domain.WorldSpawnDomain.SpawnWorldChooseScene();
            fieldEntity.SetFieldId(1);
            var fieldEntityRepo = worldFacades.Repo.FiledEntityRepo;
            fieldEntityRepo.Add(fieldEntity);

            // Send Spawn Role Message
            var rqs = worldFacades.Network.WorldRoleReqAndRes;
            rqs.SendReq_WolrdRoleSpawn(worldClientFrameIndex);
        }




    }

}
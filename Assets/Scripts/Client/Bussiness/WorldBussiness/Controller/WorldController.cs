using System.Collections.Generic;
using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {
        WorldFacades worldFacades;
        int worldClientFrameIndex;
        int worldClientFrameIndex_Ahead;

        // 操作队列
        Queue<FrameOptResMsg> optQueue;
        List<FrameOptResResendMsg> resendList_Opt;

        // 生成队列
        Queue<FrameWRoleSpawnResMsg> spawnQueue;
        List<FrameWRoleSpawnResResendMsg> resendList_Spawn;

        // 人物状态同步队列
        Queue<WRoleStateUpdateMsg> stateQueue;

        public WorldController()
        {
            // Between Bussiness
            NetworkEventCenter.RegistLoginSuccess(EnterWorldChooseScene);

            optQueue = new Queue<FrameOptResMsg>();
            resendList_Opt = new List<FrameOptResResendMsg>();

            spawnQueue = new Queue<FrameWRoleSpawnResMsg>();
            resendList_Spawn = new List<FrameWRoleSpawnResResendMsg>();

            stateQueue = new Queue<WRoleStateUpdateMsg>();
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
            var req = worldFacades.Network.WorldRoleReqAndRes;
            // req.RegistRes_WorldRoleOpt(OnWorldRoleOpt);
            // req.RegistResResend_WorldRoleSpawn(OnWorldRoleSpawnResend);
            // req.RegistResResend_Opt(OnWorldRoleOptResend);
            req.RegistRes_WorldRoleSpawn(OnWorldRoleSpawn);
            req.RegistUpdate_WRole(OnWRoleSync);
        }

        public void Tick()
        {
            Tick_Input();
            Tick_ServerResQueues();
        }

        void Tick_ServerResQueues()
        {
            int nextFrameIndex = worldClientFrameIndex + 1;
            // if (optQueue.TryPeek(out var opt) && nextFrameIndex == opt.serverFrameIndex)
            // {
            //     optQueue.Dequeue();
            //     worldClientFrameIndex = nextFrameIndex;
            //     Debug.Log($"操作帧 : {worldClientFrameIndex}");

            //     var optTypeId = opt.optTypeId;
            //     // 解析操作
            //     if (optTypeId == 1)
            //     {
            //         //移动操作
            //         var realMsg = opt.msg;
            //         var rid = (byte)(realMsg >> 24);
            //         Vector3 dir = new Vector3((sbyte)(realMsg >> 16), (sbyte)(realMsg >> 8), (sbyte)realMsg);
            //         var roleEntity = worldFacades.Repo.WorldRoleRepo.Get(rid);

            //         roleEntity.MoveComponent.Move(dir);
            //     }
            // }

            if (spawnQueue.TryPeek(out var spawn) && nextFrameIndex == spawn.serverFrameIndex)
            {
                spawnQueue.Dequeue();
                worldClientFrameIndex = nextFrameIndex;
                worldClientFrameIndex_Ahead = worldClientFrameIndex;
                Debug.Log($"生成帧 : {worldClientFrameIndex}");
                var wRoleId = spawn.wRoleId;
                var repo = worldFacades.Repo;
                var fieldEntity = repo.FiledEntityRepo.Get(1);
                var domain = worldFacades.Domain.WorldRoleSpawnDomain;
                var entity = domain.SpawnWorldRole(fieldEntity.transform);
                entity.SetWRid(wRoleId);

                var roleRepo = repo.WorldRoleRepo;
                roleRepo.Add(entity);
                if (spawn.isOwner)
                {
                    roleRepo.SetOwner(entity);
                    worldFacades.CinemachineExtra.FollowSolo(entity.transform, 3f);
                    // worldFacades.CinemachineExtra.LookAtSolo(entity.CamTrackingObj, 3f);
                }

                Debug.Log(spawn.isOwner ? $"生成自身角色 : WRid:{entity.WRid}" : $"生成其他角色 : WRid:{entity.WRid}");
            }

            if (stateQueue.TryPeek(out var stateMsg))
            {
                stateQueue.Dequeue();

                worldClientFrameIndex = stateMsg.serverFrameIndex;
                worldClientFrameIndex_Ahead = worldClientFrameIndex;

                RoleState roleState = (RoleState)stateMsg.roleState;
                float x = stateMsg.x / 10000f;
                float y = stateMsg.y / 10000f;
                float z = stateMsg.z / 10000f;
                float eulerX = stateMsg.eulerX / 10000f;
                float eulerY = stateMsg.eulerY / 10000f;
                float eulerZ = stateMsg.eulerZ / 10000f;
                float velocityX = stateMsg.velocityX / 10000f;
                float velocityY = stateMsg.velocityY / 10000f;
                float velocityZ = stateMsg.velocityZ / 10000f;

                Vector3 pos = new Vector3(x, y, z);
                Vector3 eulerAngle = new Vector3(eulerX, eulerY, eulerZ);
                Vector3 velocity = new Vector3(velocityX, velocityY, velocityZ);

                var entity = worldFacades.Repo.WorldRoleRepo.Get(stateMsg.wRid);
                if (entity == null)
                {
                    Debug.Log($"人物状态同步帧(entity丢失，重新生成)");

                    var wRoleId = stateMsg.wRid;
                    var repo = worldFacades.Repo;
                    var fieldEntity = repo.FiledEntityRepo.Get(1);
                    var domain = worldFacades.Domain.WorldRoleSpawnDomain;
                    entity = domain.SpawnWorldRole(fieldEntity.transform);
                    entity.SetWRid(wRoleId);

                    var roleRepo = repo.WorldRoleRepo;
                    roleRepo.Add(entity);
                    if (stateMsg.isOwner && roleRepo.Owner == null)
                    {
                        Debug.Log($"生成Owner  wRid:{entity.WRid})");
                        roleRepo.SetOwner(entity);
                        worldFacades.CinemachineExtra.FollowSolo(entity.transform, 3f);
                        worldFacades.CinemachineExtra.LookAtSolo(entity.CamTrackingObj, 3f);
                    }
                }

                Debug.Log($"人物状态同步帧 : {worldClientFrameIndex}    wRid:{stateMsg.wRid}  人物状态：{roleState.ToString()}  位置: {pos} 旋转角:{eulerAngle}");

                if (entity.RoleStatus != roleState)
                {
                    switch (roleState)
                    {
                        case RoleState.Idle:
                            entity.AnimatorComponent.PlayIdle();
                            break;
                        case RoleState.Move:
                            entity.AnimatorComponent.PlayRun();
                            break;
                        case RoleState.Jump:
                            entity.MoveComponent.Jump();
                            entity.AnimatorComponent.PlayRun();
                            break;
                    }

                    entity.SetRoleStatus(roleState);
                }

                //判断是否回滚预测操作
                var moveComponent = entity.MoveComponent;
                var lastSyncFramePos = moveComponent.LastSyncFramePos;
                if (!lastSyncFramePos.Equals(pos, 2))
                {
                    moveComponent.SetCurPos(pos);
                    moveComponent.UpdateLastSyncFramePos();
                    Debug.Log($"校准位置:FramePos:    {lastSyncFramePos}   ------>   {moveComponent.LastSyncFramePos}");
                    moveComponent.SetVelocity(velocity);
                    Debug.Log($"校准速度   ------>   {velocity}");
                }
                if (!moveComponent.EulerAngel.Equals(eulerAngle, 2))
                {
                    Debug.Log($"校准旋转角度:EulerAngel:    {moveComponent.EulerAngel}   ------>   {eulerAngle}");
                    moveComponent.SetRotaionEulerAngle(eulerAngle);
                }


            }

        }

        void Tick_Input()
        {
            if (worldClientFrameIndex != worldClientFrameIndex_Ahead)
                return;

            //没有角色就没有移动
            var owner = worldFacades.Repo.WorldRoleRepo.Owner;
            if (owner == null) return;

            bool needMove = false;
            bool needJump = false;

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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                needJump = true;
            }

            if (needMove && !WillHitOtherRole(owner, moveDir))
            {
                byte rid = owner.WRid;
                worldFacades.Network.WorldRoleReqAndRes.SendReq_WRoleMove(worldClientFrameIndex, rid, moveDir);

                //预测操作
                owner.MoveComponent.Move(moveDir);
                owner.MoveComponent.FaceTo(moveDir);
                worldClientFrameIndex_Ahead++;
            }

            if (needJump)
            {
                byte rid = owner.WRid;
                worldFacades.Network.WorldRoleReqAndRes.SendReq_WRoleJump(worldClientFrameIndex, rid);

                //预测操作
                owner.MoveComponent.Jump();
                owner.AnimatorComponent.PlayRun();
                owner.SetRoleStatus(RoleState.Jump);
                worldClientFrameIndex_Ahead++;
            }
        }

        bool WillHitOtherRole(WorldRoleEntity roleEntity, Vector3 moveDir)
        {
            var roleRepo = worldFacades.Repo.WorldRoleRepo;
            var array = roleRepo.GetAll();
            for (int i = 0; i < array.Length; i++)
            {
                var r = array[i];
                if (r.WRid == roleEntity.WRid) continue;

                var pos1 = r.MoveComponent.CurPos;
                var pos2 = roleEntity.MoveComponent.CurPos;
                if (Vector3.Distance(pos1, pos2) < 1f)
                {
                    var betweenV = pos1 - pos2;
                    betweenV.Normalize();
                    moveDir.Normalize();
                    var cosVal = Vector3.Dot(moveDir, betweenV);
                    Debug.Log(cosVal);
                    if (cosVal > 0) return true;
                }
            }

            return false;
        }

        // == Server Response ==
        // ROLE STATE SYNC
        void OnWRoleSync(WRoleStateUpdateMsg msg)
        {
            stateQueue.Enqueue(msg);
        }

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
using System.Collections.Generic;
using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;
using Game.Client.Bussiness.EventCenter;
using System;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {
        WorldFacades worldFacades;
        int worldClientFrame;

        // 生成队列
        Queue<FrameWRoleSpawnResMsg> roleSpawnQueue;
        Queue<FrameBulletSpawnResMsg> bulletSpawnQueue;
        // 物理事件队列
        Queue<FrameBulletHitRoleResMsg> bulletHitRoleQueue;

        // 人物状态同步队列
        Queue<WRoleStateUpdateMsg> stateQueue;

        public WorldController()
        {
            // Between Bussiness
            NetworkEventCenter.RegistLoginSuccess(EnterWorldChooseScene);

            roleSpawnQueue = new Queue<FrameWRoleSpawnResMsg>();
            bulletSpawnQueue = new Queue<FrameBulletSpawnResMsg>();
            stateQueue = new Queue<WRoleStateUpdateMsg>();
            bulletHitRoleQueue = new Queue<FrameBulletHitRoleResMsg>();
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
            var req = worldFacades.Network.WorldRoleReqAndRes;
            req.RegistRes_WorldRoleSpawn(OnWorldRoleSpawn);
            req.RegistRes_BulletSpawn(OnBulletSpawn);
            req.RegistRes_BulletHitRole(OnBulletHitRole);
            req.RegistUpdate_WRole(OnWRoleSync);
        }

        public void Tick()
        {
            Tick_Input();
            Tick_ServerResQueues();
        }

        void Tick_ServerResQueues()
        {
            int nextFrame = worldClientFrame + 1;

            if (roleSpawnQueue.TryPeek(out var spawn) && nextFrame == spawn.serverFrame)
            {
                roleSpawnQueue.Dequeue();
                worldClientFrame = nextFrame;

                Debug.Log($"生成人物帧 : {worldClientFrame}");
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

            if (bulletSpawnQueue.TryPeek(out var bulletSpawn) && nextFrame == bulletSpawn.serverFrame)
            {
                bulletSpawnQueue.Dequeue();
                worldClientFrame = nextFrame;

                var bulletId = bulletSpawn.bulletId;
                var bulletType = bulletSpawn.bulletType;
                var masterWRid = bulletSpawn.wRid;
                var masterWRole = worldFacades.Repo.WorldRoleRepo.Get(masterWRid);
                var shootStartPoint = masterWRole.ShootPointPos;
                Vector3 shootDir = new Vector3(bulletSpawn.shootDirX / 100f, bulletSpawn.shootDirY / 100f, bulletSpawn.shootDirZ / 100f);
                Debug.Log($"生成子弹帧 {worldClientFrame}: masterWRid:{masterWRid}   起点位置：{shootStartPoint} 飞行方向{shootDir}");
                var fieldEntity = worldFacades.Repo.FiledEntityRepo.Get(1);
                var bulletEntity = worldFacades.Domain.BulletSpawnDomain.SpawnBullet(fieldEntity.transform, (BulletType)bulletType);
                bulletEntity.MoveComponent.SetCurPos(shootStartPoint);
                bulletEntity.MoveComponent.Move(shootDir);
                bulletEntity.SetWRid(masterWRid);
                bulletEntity.SetBulletId(bulletId);
                var bulletRepo = worldFacades.Repo.BulletEntityRepo;
                bulletRepo.Add(bulletEntity);
            }

            if (stateQueue.TryPeek(out var stateMsg))
            {
                stateQueue.Dequeue();

                worldClientFrame = stateMsg.serverFrameIndex;

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
                var log = $"人物状态同步帧 : {worldClientFrame}  wRid:{stateMsg.wRid}  人物状态：{roleState.ToString()}  位置: {pos} 旋转角:{eulerAngle} ";
                Debug.Log($"<color=#ff0000>{log}</color>");

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

            if (bulletHitRoleQueue.TryPeek(out var bulletHitRole) && nextFrame == bulletHitRole.serverFrame)
            {
                bulletHitRoleQueue.Dequeue();
                worldClientFrame = nextFrame;

                var bulletRepo = worldFacades.Repo.BulletEntityRepo;
                var roleRepo = worldFacades.Repo.WorldRoleRepo;
                var bullet = bulletRepo.GetByBulletId(bulletHitRole.bulletId);
                var role = roleRepo.Get(bulletHitRole.wRid);

                role.HealthComponent.HurtByBullet(bullet);
                role.MoveComponent.HitByBullet(bullet);
                if (role.HealthComponent.IsDead)
                {
                    role.TearDown();
                    role.Reborn();
                }

                GameObject.Destroy(bullet.gameObject);
            }
        }

        void Tick_Input()
        {
            //没有角色就没有移动
            var owner = worldFacades.Repo.WorldRoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            bool needMove = false;
            bool needJump = false;

            bool needShoot = false;
            BulletType bulletType = default;

            Vector3 moveDir = Vector3.zero;
            Vector3 targetPos = Vector3.zero;
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
            if (Input.GetKeyDown(KeyCode.G))
            {
                needShoot = true;
                bulletType = BulletType.Grenade;
            }
            if (Input.GetMouseButtonDown(0))
            {
                needShoot = true;
                targetPos = owner.ShootPointPos + owner.transform.forward;
                bulletType = BulletType.Default;
            }

            if (needMove && !WillHitOtherRole(owner, moveDir))
            {
                byte rid = owner.WRid;
                worldFacades.Network.WorldRoleReqAndRes.SendReq_WRoleMove(worldClientFrame, rid, moveDir);

                //预测操作
                owner.MoveComponent.Move(moveDir);
                owner.MoveComponent.FaceTo(moveDir);
            }

            if (needJump)
            {
                byte rid = owner.WRid;
                worldFacades.Network.WorldRoleReqAndRes.SendReq_WRoleJump(worldClientFrame, rid);
                //预测操作
                owner.MoveComponent.Jump();
                owner.AnimatorComponent.PlayRun();
                owner.SetRoleStatus(RoleState.Jump);
            }

            if (needShoot)
            {
                byte rid = owner.WRid;
                worldFacades.Network.BulletReqAndRes.SendReq_BulletSpawn(worldClientFrame, bulletType, rid, targetPos);
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
        void OnWorldRoleSpawn(FrameWRoleSpawnResMsg msg)
        {
            // Debug.Log("加入角色生成队列");
            roleSpawnQueue.Enqueue(msg);
        }
        void OnBulletSpawn(FrameBulletSpawnResMsg msg)
        {
            // Debug.Log("加入子弹生成队列");
            bulletSpawnQueue.Enqueue(msg);
        }

        // PHYSICS
        void OnBulletHitRole(FrameBulletHitRoleResMsg msg)
        {
            // Debug.Log("加入子弹生成队列");
            bulletHitRoleQueue.Enqueue(msg);
        }

        // Network Event Center
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
            rqs.SendReq_WolrdRoleSpawn(worldClientFrame);
        }

    }

}
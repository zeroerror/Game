using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;
using Game.Client.Bussiness.EventCenter;
using ZeroFrame.ZeroMath;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {
        WorldFacades worldFacades;
        int worldClientFrame;
        float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;
        // 服务器下发的生成队列
        Queue<FrameWRoleSpawnResMsg> roleSpawnQueue;
        Queue<FrameBulletSpawnResMsg> bulletSpawnQueue;
        // 服务器下发的物理事件队列
        Queue<FrameBulletHitRoleResMsg> bulletHitRoleQueue;
        Queue<FrameBulletHitWallResMsg> bulletHitWallQueue;
        Queue<FrameBulletTearDownResMsg> bulletTearDownQueue;

        // 服务器下发的人物状态同步队列
        Queue<WRoleStateUpdateMsg> stateQueue;

        bool isSync;

        public WorldController()
        {
            // Between Bussiness
            NetworkEventCenter.RegistLoginSuccess(EnterWorldChooseScene);

            roleSpawnQueue = new Queue<FrameWRoleSpawnResMsg>();
            bulletSpawnQueue = new Queue<FrameBulletSpawnResMsg>();

            bulletHitRoleQueue = new Queue<FrameBulletHitRoleResMsg>();
            bulletHitWallQueue = new Queue<FrameBulletHitWallResMsg>();
            bulletTearDownQueue = new Queue<FrameBulletTearDownResMsg>();

            stateQueue = new Queue<WRoleStateUpdateMsg>();

        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;

            var roleRqs = worldFacades.Network.WorldRoleReqAndRes;
            roleRqs.RegistRes_WorldRoleSpawn(OnWorldRoleSpawn);
            roleRqs.RegistUpdate_WRole(OnWRoleSync);

            var bulletRqs = worldFacades.Network.BulletReqAndRes;
            bulletRqs.RegistRes_BulletSpawn(OnBulletSpawn);
            bulletRqs.RegistRes_BulletHitRole(OnBulletHitRole);
            bulletRqs.RegistRes_BulletHitWall(OnBulletHitWall);
            bulletRqs.RegistRes_BulletTearDown(OnBulletTearDown);

        }

        public void Tick()
        {
            int nextFrame = worldClientFrame + 1;

            // Physics Simulation
            if (worldFacades.Repo.FiledEntityRepo.CurFieldEntity != null)
            {
                Tick_Physics_RoleMovement(fixedDeltaTime);
            }
            Tick_Physics_BulletMovement(fixedDeltaTime);

            //1
            Tick_RoleSpawn(nextFrame);
            Tick_BulletSpawn(nextFrame);

            Tick_BulletHitRole(nextFrame);
            Tick_BulletHitWall(nextFrame);

            Tick_BulletLife(nextFrame);
            Tick_RoleStateSync(nextFrame);
            //2
            Tick_Input();

            var physicsScene = worldFacades.Repo.FiledEntityRepo.CurPhysicsScene;
            physicsScene.Simulate(fixedDeltaTime);
        }

        public void RendererUpdate()
        {
            // 相机朝向更新
            var owner = worldFacades.Repo.WorldRoleRepo.Owner;
            if (owner != null)
            {
                var axisX = Input.GetAxis("Mouse X");
                var axisY = -Input.GetAxis("Mouse Y");
                owner.MoveComponent.AddEulerAngleY(axisX);

                var curCamComponent = worldFacades.Repo.FiledEntityRepo.CurFieldEntity.CameraComponent;
                var curCam = curCamComponent.CurrentCamera;
                var ownerEulerAngle = owner.transform.rotation.eulerAngles;
                switch (curCamComponent.CurrentCameraView)
                {
                    case CameraView.FirstView:
                        curCam.AddEulerAngleX(axisY);
                        curCam.SetEulerAngleY(ownerEulerAngle.y);
                        break;
                    case CameraView.ThirdView:
                        // curCam.SetEulerAngleY(ownerEulerAngle.y);
                        break;
                }
            }
        }
        void Tick_RoleStateSync(int nextFrame)
        {
            while (stateQueue.TryPeek(out var stateMsg))
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
                float moveVelocityX = stateMsg.moveVelocityX / 10000f;
                float moveVelocityY = stateMsg.moveVelocityY / 10000f;
                float moveVelocityZ = stateMsg.moveVelocityZ / 10000f;
                float extraVelocityX = stateMsg.extraVelocityX / 10000f;
                float extraVelocityY = stateMsg.extraVelocityY / 10000f;
                float extraVelocityZ = stateMsg.extraVelocityZ / 10000f;
                float gravityVelocity = stateMsg.gravityVelocity / 10000f;

                Vector3 pos = new Vector3(x, y, z);
                Vector3 eulerAngle = new Vector3(eulerX, eulerY, eulerZ);
                Vector3 moveVelocity = new Vector3(moveVelocityX, moveVelocityY, moveVelocityZ);
                Vector3 extraVelocity = new Vector3(extraVelocityX, extraVelocityY, extraVelocityZ);

                var repo = worldFacades.Repo;
                var roleRepo = repo.WorldRoleRepo;
                var fieldRepo = repo.FiledEntityRepo;
                var role = worldFacades.Repo.WorldRoleRepo.Get(stateMsg.wRid);
                if (role == null)
                {
                    Debug.Log($"人物状态同步帧(entity丢失，重新生成)");

                    var wRoleId = stateMsg.wRid;
                    var fieldEntity = fieldRepo.Get(1);
                    var domain = worldFacades.Domain.WorldRoleSpawnDomain;
                    role = domain.SpawnWorldRole(fieldEntity.transform);
                    role.SetWRid(wRoleId);

                    roleRepo.Add(role);
                    if (stateMsg.isOwner && roleRepo.Owner == null)
                    {
                        Debug.Log($"生成Owner  wRid:{role.WRid})");
                        roleRepo.SetOwner(role);
                        var fieldCameraComponent = fieldEntity.CameraComponent;
                        fieldCameraComponent.OpenThirdViewCam(role);
                    }
                }
                var log = $"人物状态同步帧 : {worldClientFrame}  wRid:{stateMsg.wRid} 角色状态:{roleState.ToString()} 位置 :{pos} 移动速度：{moveVelocity} 额外速度：{extraVelocity}  重力速度:{gravityVelocity}  旋转角度：{eulerAngle}";
                Debug.Log($"<color=#008000>{log}</color>");

                switch (roleState)
                {
                    case RoleState.Normal:
                        role.AnimatorComponent.PlayIdle();
                        break;
                    case RoleState.Move:
                        role.AnimatorComponent.PlayRun();
                        break;
                    case RoleState.Jump:
                        if (role.RoleState != RoleState.Jump)
                        {
                            role.AnimatorComponent.PlayJump();
                        }
                        role.MoveComponent.SetJumpVelocity();
                        break;
                    case RoleState.Hooking:
                        role.AnimatorComponent.PlayHooking();
                        break;
                }
                role.MoveComponent.SetCurPos(pos);
                if (roleRepo.Owner.WRid != role.WRid) role.MoveComponent.SetEulerAngle(eulerAngle);
                role.MoveComponent.SetMoveVelocity(moveVelocity);
                role.MoveComponent.SetExtraVelocity(extraVelocity);
                role.MoveComponent.SetGravityVelocity(gravityVelocity);

                role.SetRoleState(roleState);
            }
        }

        void Tick_Input()
        {
            //没有角色就没有移动
            var owner = worldFacades.Repo.WorldRoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = worldFacades.InputComponent;
            if (input.pressJump)
            {
                byte rid = owner.WRid;
                worldFacades.Network.WorldRoleReqAndRes.SendReq_WRoleJump(worldClientFrame, rid);
            }
            if (input.pressV)
            {
                //打开第一人称视角
                // TODO: 加切换视角的判定条件
                var fieldCameraComponent = worldFacades.Repo.FiledEntityRepo.CurFieldEntity.CameraComponent;
                if (fieldCameraComponent.CurrentCameraView == CameraView.ThirdView) fieldCameraComponent.OpenFirstViewCam(owner);
                else if (fieldCameraComponent.CurrentCameraView == CameraView.FirstView) fieldCameraComponent.OpenThirdViewCam(owner);
            }
            if (input.shootPoint != Vector3.zero)
            {
                // TODO: 是否满足条件
                byte rid = owner.WRid;
                worldFacades.Network.BulletReqAndRes.SendReq_BulletSpawn(worldClientFrame, BulletType.Default, rid, input.shootPoint);
            }

            if (input.grenadeThrowPoint != Vector3.zero)
            {
                // TODO: 是否满足条件
                byte rid = owner.WRid;
                worldFacades.Network.BulletReqAndRes.SendReq_BulletSpawn(worldClientFrame, BulletType.Grenade, rid, input.grenadeThrowPoint);
            }

            if (input.hookPoint != Vector3.zero)
            {
                // TODO: 是否满足条件
                byte rid = owner.WRid;
                worldFacades.Network.BulletReqAndRes.SendReq_BulletSpawn(worldClientFrame, BulletType.Hooker, rid, input.hookPoint);
            }

            if (input.moveAxis != Vector3.zero)
            {
                var moveAxis = input.moveAxis;
                Vector3 moveDir = moveAxis;
                var currentCamView = worldFacades.Repo.FiledEntityRepo.CurFieldEntity.CameraComponent.CurrentCameraView;
                if (currentCamView == CameraView.FirstView)
                {
                    Vector3 roleForward = owner.transform.forward;
                    roleForward.y = 0;
                    VectorHelper2D.GetRotVector(roleForward.x, roleForward.z, -90, out float rightX, out float rightZ);
                    Vector3 roleRight = new Vector3(rightX, 0, rightZ);
                    moveDir.x *= roleForward.x;
                    moveDir = moveAxis.z * roleForward; //前后
                    moveDir += moveAxis.x * roleRight;  //左右
                }
                if (!WillHitOtherRole(owner, moveDir))
                {
                    var rqs = worldFacades.Network.WorldRoleReqAndRes;
                    if (owner.IsEulerAngleNeedFlush())
                    {
                        owner.FlushEulerAngle();
                        //客户端鉴权旋转角度同步
                        rqs.SendReq_WRoleRotate(worldClientFrame, owner);
                    }

                    byte rid = owner.WRid;
                    rqs.SendReq_WRoleMove(worldClientFrame, rid, moveDir);
                }
            }

            input.Reset();

            if (owner.IsEulerAngleNeedFlush())
            {
                owner.FlushEulerAngle();
                //客户端鉴权旋转角度同步
                var rqs = worldFacades.Network.WorldRoleReqAndRes;
                rqs.SendReq_WRoleRotate(worldClientFrame, owner);
            }

        }

        void Tick_BulletLife(int nextFrame)
        {
            while (bulletTearDownQueue.TryDequeue(out var msg))
            {
                var bulletId = msg.bulletId;
                var bulletType = msg.bulletType;
                var bulletRepo = worldFacades.Repo.BulletEntityRepo;
                var bulletEntity = bulletRepo.GetByBulletId(bulletId);

                Vector3 pos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);
                bulletEntity.MoveComponent.SetCurPos(pos);

                if (bulletEntity.BulletType == BulletType.Default)
                {
                    bulletEntity.TearDown();
                }

                if (bulletEntity.BulletType == BulletType.Grenade)
                {
                    ((GrenadeEntity)bulletEntity).TearDown();
                }

                if (bulletEntity.BulletType == BulletType.Hooker)
                {
                    ((HookerEntity)bulletEntity).TearDown();
                }

                bulletRepo.TryRemove(bulletEntity);
            }
        }

        void Tick_Physics_RoleMovement(float deltaTime)
        {
            var domain = worldFacades.Domain.WorldRoleSpawnDomain;
            domain.Tick_RoleRigidbody(deltaTime);
        }

        void Tick_Physics_BulletMovement(float fixedDeltaTime)
        {
            var domain = worldFacades.Domain.BulletDomain;
            domain.Tick_Bullet(fixedDeltaTime);
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

        void Tick_RoleSpawn(int nextFrame)
        {
            if (roleSpawnQueue.TryPeek(out var spawn))
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

                var fieldCameraComponent = fieldEntity.CameraComponent;
                if (spawn.isOwner)
                {
                    roleRepo.SetOwner(entity);
                    fieldCameraComponent.OpenThirdViewCam(entity);
                }

                Debug.Log(spawn.isOwner ? $"生成自身角色 : WRid:{entity.WRid}" : $"生成其他角色 : WRid:{entity.WRid}");
            }
        }

        void Tick_BulletSpawn(int nextFrame)
        {
            if (bulletSpawnQueue.TryPeek(out var bulletSpawn))
            {
                bulletSpawnQueue.Dequeue();
                worldClientFrame = nextFrame;

                var bulletId = bulletSpawn.bulletId;
                var bulletTypeByte = bulletSpawn.bulletType;
                var bulletType = (BulletType)bulletTypeByte;
                var masterWRid = bulletSpawn.wRid;
                var masterWRole = worldFacades.Repo.WorldRoleRepo.Get(masterWRid);
                var shootStartPoint = masterWRole.ShootPointPos;
                Vector3 shootDir = new Vector3(bulletSpawn.shootDirX / 100f, bulletSpawn.shootDirY / 100f, bulletSpawn.shootDirZ / 100f);
                Debug.Log($"生成子弹帧 {worldClientFrame}: masterWRid:{masterWRid}   起点位置：{shootStartPoint} 飞行方向{shootDir}");
                var fieldEntity = worldFacades.Repo.FiledEntityRepo.Get(1);
                var bulletEntity = worldFacades.Domain.BulletDomain.SpawnBullet(fieldEntity.transform, bulletType);
                switch (bulletType)
                {
                    case BulletType.Default:
                        break;
                    case BulletType.Grenade:
                        break;
                    case BulletType.Hooker:
                        var hookerEntity = (HookerEntity)bulletEntity;
                        hookerEntity.SetMasterWRid(masterWRid);
                        hookerEntity.SetMasterGrabPoint(masterWRole.transform);
                        break;
                }
                bulletEntity.MoveComponent.SetCurPos(shootStartPoint);
                bulletEntity.MoveComponent.SetForward(shootDir);
                bulletEntity.MoveComponent.AddMoveVelocity(shootDir);
                bulletEntity.SetWRid(masterWRid);
                bulletEntity.SetBulletId(bulletId);
                var bulletRepo = worldFacades.Repo.BulletEntityRepo;
                bulletRepo.Add(bulletEntity);
            }
        }

        void Tick_BulletHitRole(int nextFrame)
        {
            while (bulletHitRoleQueue.TryPeek(out var bulletHitRoleMsg))
            {
                bulletHitRoleQueue.Dequeue();
                worldClientFrame = nextFrame;

                var bulletRepo = worldFacades.Repo.BulletEntityRepo;
                var roleRepo = worldFacades.Repo.WorldRoleRepo;
                var bullet = bulletRepo.GetByBulletId(bulletHitRoleMsg.bulletId);
                var role = roleRepo.Get(bulletHitRoleMsg.wRid);

                // Client Logic
                role.HealthComponent.HurtByBullet(bullet);
                role.MoveComponent.HitByBullet(bullet);
                if (role.HealthComponent.IsDead)
                {
                    role.TearDown();
                    role.Reborn();
                }

                if (bullet is HookerEntity hookerEntity)
                {
                    // 如果是爪钩则是抓住某物而不是销毁
                    hookerEntity.TryGrabSomthing(role.transform);
                    continue;
                }

                bullet.TearDown();
                bulletRepo.TryRemove(bullet);
            }
        }

        void Tick_BulletHitWall(int nextFrame)
        {
            while (bulletHitWallQueue.TryPeek(out var bulletHitWallResMsg))
            {
                bulletHitWallQueue.Dequeue();
                worldClientFrame = nextFrame;

                var bulletHitPos = new Vector3(bulletHitWallResMsg.posX / 10000f, bulletHitWallResMsg.posY / 10000f, bulletHitWallResMsg.posZ / 10000f);
                var bulletRepo = worldFacades.Repo.BulletEntityRepo;
                var roleRepo = worldFacades.Repo.WorldRoleRepo;
                var bullet = bulletRepo.GetByBulletId(bulletHitWallResMsg.bulletId);

                if (bullet is HookerEntity hookerEntity)
                {
                    // 如果是爪钩则是抓住某物而不是销毁
                    hookerEntity.TryGrabSomthing(bulletHitPos);
                    continue;
                }

                bullet.TearDown();
                bulletRepo.TryRemove(bullet);
            }
        }

        // == Server Response ==
        // ROLE 
        void OnWRoleSync(WRoleStateUpdateMsg msg)
        {
            stateQueue.Enqueue(msg);
        }

        void OnWorldRoleSpawn(FrameWRoleSpawnResMsg msg)
        {
            // Debug.Log("加入角色生成队列");
            roleSpawnQueue.Enqueue(msg);
        }

        // BULLET
        void OnBulletSpawn(FrameBulletSpawnResMsg msg)
        {
            Debug.Log($"加入子弹生成队列");
            bulletSpawnQueue.Enqueue(msg);
        }

        void OnBulletHitRole(FrameBulletHitRoleResMsg msg)
        {
            Debug.Log("加入子弹击中角色队列");
            bulletHitRoleQueue.Enqueue(msg);
        }

        void OnBulletHitWall(FrameBulletHitWallResMsg msg)
        {
            Debug.Log("加入子弹击中墙壁队列");
            bulletHitWallQueue.Enqueue(msg);
        }

        void OnBulletTearDown(FrameBulletTearDownResMsg msg)
        {
            Debug.Log($"{msg.bulletType.ToString()} 加入子弹销毁队列");
            bulletTearDownQueue.Enqueue(msg);
        }

        // Network Event Center
        async void EnterWorldChooseScene()
        {
            // 当前有加载好的场景，则不加载
            var curFieldEntity = worldFacades.Repo.FiledEntityRepo.CurFieldEntity;
            if (curFieldEntity != null) return;

            // Load Scene And Spawn Field
            var domain = worldFacades.Domain;
            var fieldEntity = await domain.WorldSpawnDomain.SpawnWorldChooseScene();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            fieldEntity.SetFieldId(1);
            var fieldEntityRepo = worldFacades.Repo.FiledEntityRepo;
            var physicsScene = fieldEntity.gameObject.scene.GetPhysicsScene();
            fieldEntityRepo.Add(fieldEntity);
            fieldEntityRepo.SetPhysicsScene(physicsScene);
            // Send Spawn Role Message
            var rqs = worldFacades.Network.WorldRoleReqAndRes;
            rqs.SendReq_WolrdRoleSpawn(worldClientFrame);
        }

    }

}
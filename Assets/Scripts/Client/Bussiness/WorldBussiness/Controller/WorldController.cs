using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;
using Game.Client.Bussiness.EventCenter;
using ZeroFrame.ZeroMath;
using Game.Generic;
using Game.Client.Bussiness.WorldBussiness.Interface;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {
        WorldFacades worldFacades;
        int clientFrame;
        float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;
        // 服务器下发的生成队列
        Queue<FrameWRoleSpawnResMsg> roleSpawnQueue;
        Queue<FrameBulletSpawnResMsg> bulletSpawnQueue;
        Queue<FrameItemSpawnResMsg> itemSpawnQueue;
        // 服务器下发的物理事件队列
        Queue<FrameBulletHitRoleResMsg> bulletHitRoleQueue;
        Queue<FrameBulletHitWallResMsg> bulletHitWallQueue;
        Queue<FrameBulletTearDownResMsg> bulletTearDownQueue;
        // 服务器下发的人物状态同步队列
        Queue<WRoleStateUpdateMsg> stateQueue;
        // 服务器下发的资源拾取队列
        Queue<FrameItemPickResMsg> itemPickQueue;

        public WorldController()
        {
            // Between Bussiness
            NetworkEventCenter.RegistLoginSuccess(EnterWorldChooseScene);

            roleSpawnQueue = new Queue<FrameWRoleSpawnResMsg>();
            bulletSpawnQueue = new Queue<FrameBulletSpawnResMsg>();
            itemSpawnQueue = new Queue<FrameItemSpawnResMsg>();

            bulletHitRoleQueue = new Queue<FrameBulletHitRoleResMsg>();
            bulletHitWallQueue = new Queue<FrameBulletHitWallResMsg>();
            bulletTearDownQueue = new Queue<FrameBulletTearDownResMsg>();

            stateQueue = new Queue<WRoleStateUpdateMsg>();

            itemPickQueue = new Queue<FrameItemPickResMsg>();
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

            var weaponRqs = worldFacades.Network.WeaponReqAndRes;
            weaponRqs.RegistRes_WeaponAssetsSpawn(OnItemSpawn);

            var ItemReqAndRes = worldFacades.Network.ItemReqAndRes;
            ItemReqAndRes.RegistRes_ItemPickUp(OnItemPickUp);
        }


        public void Tick()
        {
            int nextFrame = clientFrame + 1;
            float deltaTime = UnityEngine.Time.deltaTime;

            // == Movement
            if (worldFacades.Repo.FiledEntityRepo.CurFieldEntity != null)
            {
                Tick_Physics_Movement_Role(fixedDeltaTime);
                Tick_Physics_Movement_Bullet(fixedDeltaTime);
            }

            // == Physics Simulation
            var physicsScene = worldFacades.Repo.FiledEntityRepo.CurPhysicsScene;
            physicsScene.Simulate(fixedDeltaTime);

            // == Physics Collision(Only For Client Performances Like Hit Effect,etc.)
            Tick_Physics_Collision_Role(fixedDeltaTime);
            Tick_Physics_Collision_Bullet();
            // == Physics Server Responses
            Tick_BulletHitWall(nextFrame);
            Tick_BulletHitRole(nextFrame);

            // == Tick Server Resonse (Order By Life Cycle)
            Tick_RoleSpawn(nextFrame);
            Tick_BulletSpawn(nextFrame);
            Tick_ItemAssetsSpawn(nextFrame);

            Tick_ItemPick(nextFrame);

            Tick_RoleStateSync(nextFrame);
            Tick_BulletTearDown(nextFrame);

            // == Input
            Tick_Input();
        }

        #region [Input]

        void Tick_Input()
        {
            //没有角色就没有移动
            var owner = worldFacades.Repo.WorldRoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = worldFacades.InputComponent;
            if (input.isPressJump)
            {
                byte rid = owner.WRid;
                worldFacades.Network.WorldRoleReqAndRes.SendReq_WRoleJump(clientFrame, rid);
            }
            if (input.isPressSwitchView)
            {
                //打开第一人称视角
                // TODO: 加切换视角的判定条件
                var fieldCameraComponent = worldFacades.Repo.FiledEntityRepo.CurFieldEntity.CameraComponent;
                if (fieldCameraComponent.CurrentCameraView == CameraView.ThirdView) fieldCameraComponent.OpenFirstViewCam(owner.roleRenderer);
                else if (fieldCameraComponent.CurrentCameraView == CameraView.FirstView) fieldCameraComponent.OpenThirdViewCam(owner.roleRenderer);
            }
            if (input.isPressShoot)
            {
                // TODO: 是否满足条件 
                var cameraView = worldFacades.Repo.FiledEntityRepo.CurFieldEntity.CameraComponent.CurrentCameraView;
                var shotPoint = worldFacades.Domain.WorldInputDomain.GetShotPointByCameraView(cameraView, owner);
                byte rid = owner.WRid;
                worldFacades.Network.BulletReqAndRes.SendReq_BulletSpawn(clientFrame, BulletType.DefaultBullet, rid, shotPoint);
            }
            if (input.isPressPickUpItem)
            {
                // TODO: 是否满足条件
                var domain = worldFacades.Domain.PhysicsDomain;
                var nearItemList = domain.GetHitItem_ColliderList(owner);
                float closestDis = float.MaxValue;
                Collider closestCollider = null;
                IPickable closestPickable = null;
                Vector3 ownerPos = owner.MoveComponent.CurPos;

                nearItemList.ForEach((item) =>
                {
                    var weaponEntity = item.collider.GetComponentInParent<WeaponEntity>();
                    if (weaponEntity == null) return;

                    var collider = item.collider;
                    var dis = Vector3.Distance(collider.transform.position, ownerPos);
                    if (dis < closestDis)
                    {
                        closestDis = dis;
                        closestCollider = item.collider;
                        closestPickable = weaponEntity;
                    }
                });

                if (closestCollider != null)
                {
                    var rqs = worldFacades.Network.ItemReqAndRes;
                    rqs.SendReq_ItemPickUp(clientFrame, owner.WRid, closestPickable.ItemType, closestPickable.EntityId);
                }

            }

            if (input.grenadeThrowPoint != Vector3.zero)
            {
                // TODO: 是否满足条件
                byte rid = owner.WRid;
                worldFacades.Network.BulletReqAndRes.SendReq_BulletSpawn(clientFrame, BulletType.Grenade, rid, input.grenadeThrowPoint);
            }

            if (input.hookPoint != Vector3.zero)
            {
                // TODO: 是否满足条件
                byte rid = owner.WRid;
                worldFacades.Network.BulletReqAndRes.SendReq_BulletSpawn(clientFrame, BulletType.Hooker, rid, input.hookPoint);
            }

            if (input.moveAxis != Vector3.zero)
            {
                var moveAxis = input.moveAxis;

                var cameraView = worldFacades.Repo.FiledEntityRepo.CurFieldEntity.CameraComponent.CurrentCameraView;
                Vector3 moveDir = worldFacades.Domain.WorldInputDomain.GetMoveDirByCameraView(owner, moveAxis, cameraView);
                owner.MoveComponent.FaceTo(moveDir);


                if (!WillHitOtherRole(owner, moveDir))
                {
                    var rqs = worldFacades.Network.WorldRoleReqAndRes;
                    if (owner.MoveComponent.IsEulerAngleNeedFlush())
                    {
                        owner.MoveComponent.FlushEulerAngle();
                        //客户端鉴权旋转角度同步
                        rqs.SendReq_WRoleRotate(clientFrame, owner);
                    }

                    byte rid = owner.WRid;
                    rqs.SendReq_WRoleMove(clientFrame, rid, moveDir);
                }
            }

            input.Reset();

            if (owner.MoveComponent.IsEulerAngleNeedFlush())
            {
                owner.MoveComponent.FlushEulerAngle();
                //客户端鉴权旋转角度同步
                var rqs = worldFacades.Network.WorldRoleReqAndRes;
                rqs.SendReq_WRoleRotate(clientFrame, owner);
            }

        }

        #endregion

        #region [Renderer]

        public void Update_RoleRenderer(float deltaTime)
        {
            var domain = worldFacades.Domain.WorldRoleDomain;
            domain.Update_RoleRenderer(deltaTime);
        }

        public void Update_Camera()
        {
            var curFieldEntity = worldFacades.Repo.FiledEntityRepo.CurFieldEntity;
            if (curFieldEntity == null) return;

            var cameraComponent = curFieldEntity.CameraComponent;
            var currentCam = cameraComponent.CurrentCamera;
            var cameraView = cameraComponent.CurrentCameraView;
            var inputDomain = worldFacades.Domain.WorldInputDomain;
            Vector2 inputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            inputDomain.UpdateCameraByCameraView(worldFacades.Repo.WorldRoleRepo.Owner, cameraView, currentCam, inputAxis);
        }

        #endregion

        #region [Physics]

        void Tick_Physics_Collision_Role(float fixedDeltaTime)
        {
            var physicsDomain = worldFacades.Domain.PhysicsDomain;
            var roleList = physicsDomain.Tick_AllRoleHitEnter(fixedDeltaTime);
        }

        void Tick_Physics_Collision_Bullet()
        {
            var physicsDomain = worldFacades.Domain.PhysicsDomain;
            physicsDomain.Refresh_BulletHit();
            // TODO:客户端这边就负责击中特效啥的
        }

        void Tick_Physics_Movement_Role(float deltaTime)
        {
            var domain = worldFacades.Domain.WorldRoleDomain;
            domain.Tick_RoleRigidbody(deltaTime);

            var cameraComponent = worldFacades.Repo.FiledEntityRepo.CurFieldEntity.CameraComponent;
            var currentCamera = cameraComponent.CurrentCamera;
            var cameraView = cameraComponent.CurrentCameraView;
            var inputDomain = worldFacades.Domain.WorldInputDomain;

            Vector2 inputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            inputDomain.UpdateCameraByCameraView(worldFacades.Repo.WorldRoleRepo.Owner, cameraView, currentCamera, inputAxis);
        }

        void Tick_Physics_Movement_Bullet(float fixedDeltaTime)
        {
            var domain = worldFacades.Domain.BulletDomain;
            domain.Tick_Bullet(fixedDeltaTime);
        }

        #endregion

        #region [Tick Server Resonse]

        #region [Role]
        void Tick_RoleStateSync(int nextFrame)
        {
            while (stateQueue.TryPeek(out var stateMsg))
            {
                stateQueue.Dequeue();
                clientFrame = stateMsg.serverFrameIndex;

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
                var roleLogic = worldFacades.Repo.WorldRoleRepo.Get(stateMsg.wRid);
                if (roleLogic == null)
                {
                    var wRoleId = stateMsg.wRid;
                    var fieldEntity = fieldRepo.Get(1);
                    var domain = worldFacades.Domain.WorldRoleDomain;
                    roleLogic = domain.SpawnWorldRoleLogic(fieldEntity.transform);
                    roleLogic.SetWRid(wRoleId);
                    roleLogic.Ctor();

                    var roleRenderer = domain.SpawnWorldRoleRenderer(fieldEntity.Role_Group_Renderer);
                    roleRenderer.SetWRid(wRoleId);
                    roleRenderer.Ctor();
                    roleLogic.Inject(roleRenderer);

                    roleRepo.Add(roleLogic);
                    if (stateMsg.isOwner && roleRepo.Owner == null)
                    {
                        Debug.Log($"生成Owner  wRid:{roleLogic.WRid})");
                        roleRepo.SetOwner(roleLogic);
                        var fieldCameraComponent = fieldEntity.CameraComponent;
                        fieldCameraComponent.OpenThirdViewCam(roleLogic.roleRenderer);
                    }
                    else
                    {
                        Debug.Log($"人物状态同步帧(roleLogic[{wRoleId}]丢失，重新生成)");
                    }
                }
                // DebugExtensions.LogWithColor($"人物状态同步帧 : {worldClientFrame}  wRid:{stateMsg.wRid} 角色状态:{roleState.ToString()} 位置 :{pos} 移动速度：{moveVelocity} 额外速度：{extraVelocity}  重力速度:{gravityVelocity}  旋转角度：{eulerAngle}","#008000");

                var animatorComponent = roleLogic.roleRenderer.AnimatorComponent;
                var moveComponent = roleLogic.MoveComponent;
                switch (roleState)
                {
                    case RoleState.Normal:
                        animatorComponent.PlayIdle();
                        break;
                    case RoleState.Move:
                        if (moveComponent.IsGrouded) animatorComponent.PlayRun();
                        break;
                    case RoleState.Jump:
                        if (roleLogic.RoleState != RoleState.Jump)
                        {
                            animatorComponent.PlayJump();
                        }
                        moveComponent.TryJump();
                        break;
                    case RoleState.Hooking:
                        animatorComponent.PlayHooking();
                        break;
                }
                moveComponent.SetCurPos(pos);
                if (roleRepo.Owner == null || roleRepo.Owner.WRid != roleLogic.WRid) moveComponent.SetEulerAngle(eulerAngle);
                moveComponent.SetMoveVelocity(moveVelocity);
                moveComponent.SetExtraVelocity(extraVelocity);
                moveComponent.SetGravityVelocity(gravityVelocity);

                roleLogic.SetRoleState(roleState);
            }
        }

        void Tick_RoleSpawn(int nextFrame)
        {
            if (roleSpawnQueue.TryPeek(out var spawn))
            {
                roleSpawnQueue.Dequeue();
                clientFrame = nextFrame;

                Debug.Log($"生成人物帧 : {clientFrame}");
                var wRoleId = spawn.wRoleId;
                var repo = worldFacades.Repo;
                var fieldEntity = repo.FiledEntityRepo.Get(1);
                var domain = worldFacades.Domain.WorldRoleDomain;
                var roleLogic = domain.SpawnWorldRoleLogic(fieldEntity.Role_Group_Logic);
                roleLogic.SetWRid(wRoleId);
                roleLogic.Ctor();

                var roleRenderer = domain.SpawnWorldRoleRenderer(fieldEntity.Role_Group_Renderer);
                roleRenderer.SetWRid(wRoleId);
                roleRenderer.Ctor();
                roleLogic.Inject(roleRenderer);

                var roleRepo = repo.WorldRoleRepo;
                roleRepo.Add(roleLogic);

                var fieldCameraComponent = fieldEntity.CameraComponent;
                if (spawn.isOwner)
                {
                    roleRepo.SetOwner(roleLogic);
                    fieldCameraComponent.OpenThirdViewCam(roleLogic.roleRenderer);
                }

                Debug.Log(spawn.isOwner ? $"生成自身角色 : WRid:{roleLogic.WRid}" : $"生成其他角色 : WRid:{roleLogic.WRid}");
            }
        }
        #endregion

        #region [Bullet]
        void Tick_BulletSpawn(int nextFrame)
        {
            if (bulletSpawnQueue.TryPeek(out var bulletSpawn))
            {
                bulletSpawnQueue.Dequeue();
                clientFrame = nextFrame;

                var bulletId = bulletSpawn.bulletId;
                var bulletTypeByte = bulletSpawn.bulletType;
                var bulletType = (BulletType)bulletTypeByte;
                var masterWRid = bulletSpawn.wRid;
                var masterWRole = worldFacades.Repo.WorldRoleRepo.Get(masterWRid);
                var shootStartPoint = masterWRole.ShootPointPos;
                Vector3 shootDir = new Vector3(bulletSpawn.shootDirX / 100f, bulletSpawn.shootDirY / 100f, bulletSpawn.shootDirZ / 100f);
                Debug.Log($"生成子弹帧 {clientFrame}: masterWRid:{masterWRid}   起点位置：{shootStartPoint} 飞行方向{shootDir}");
                var fieldEntity = worldFacades.Repo.FiledEntityRepo.Get(1);
                var bulletEntity = worldFacades.Domain.BulletDomain.SpawnBullet(fieldEntity.transform, bulletType);
                switch (bulletType)
                {
                    case BulletType.DefaultBullet:
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
                bulletEntity.MoveComponent.ActivateMoveVelocity(shootDir);
                bulletEntity.SetMasterId(masterWRid);
                bulletEntity.SetEntityId(bulletId);
                var bulletRepo = worldFacades.Repo.BulletRepo;
                bulletEntity.gameObject.SetActive(true);
                bulletRepo.Add(bulletEntity);
            }
        }

        void Tick_BulletHitRole(int nextFrame)
        {
            while (bulletHitRoleQueue.TryPeek(out var bulletHitRoleMsg))
            {
                bulletHitRoleQueue.Dequeue();
                clientFrame = nextFrame;

                var bulletRepo = worldFacades.Repo.BulletRepo;
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

                if (bullet.BulletType == BulletType.DefaultBullet)
                {
                    bullet.TearDown();
                    bulletRepo.TryRemove(bullet);
                }
                else if (bullet is HookerEntity hookerEntity)
                {
                    // 如果是爪钩则是抓住某物而不是销毁
                    hookerEntity.TryGrabSomthing(role.transform);
                    continue;
                }

            }
        }

        void Tick_BulletHitWall(int nextFrame)
        {
            while (bulletHitWallQueue.TryPeek(out var bulletHitWallResMsg))
            {
                bulletHitWallQueue.Dequeue();
                clientFrame = nextFrame;

                var bulletHitPos = new Vector3(bulletHitWallResMsg.posX / 10000f, bulletHitWallResMsg.posY / 10000f, bulletHitWallResMsg.posZ / 10000f);
                var bulletRepo = worldFacades.Repo.BulletRepo;
                var roleRepo = worldFacades.Repo.WorldRoleRepo;
                var bullet = bulletRepo.GetByBulletId(bulletHitWallResMsg.bulletId);

                if (bullet == null)
                {
                    bulletRepo.TryRemove(bullet);
                    continue;
                }

                if (bullet.BulletType == BulletType.DefaultBullet)
                {
                    bullet.TearDown();
                    bulletRepo.TryRemove(bullet);
                }
                else if (bullet is HookerEntity hookerEntity)
                {
                    // 爪钩:抓住某物而不是销毁
                    hookerEntity.TryGrabSomthing(bulletHitPos);
                }
                else if (bullet is GrenadeEntity grenadeEntity)
                {
                    // 手雷:速度清零
                    bullet.MoveComponent.SetMoveVelocity(Vector3.zero);
                }

            }
        }

        void Tick_BulletTearDown(int nextFrame)
        {
            while (bulletTearDownQueue.TryDequeue(out var msg))
            {
                var bulletId = msg.bulletId;
                var bulletType = msg.bulletType;
                var bulletRepo = worldFacades.Repo.BulletRepo;
                var bulletEntity = bulletRepo.GetByBulletId(bulletId);

                Vector3 pos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);
                bulletEntity.MoveComponent.SetCurPos(pos);

                if (bulletEntity.BulletType == BulletType.DefaultBullet)
                {
                    bulletEntity.TearDown();
                }

                if (bulletEntity is GrenadeEntity grenadeEntity)
                {
                    grenadeEntity.TearDown();
                }

                if (bulletEntity is HookerEntity hookerEntity)
                {
                    hookerEntity.TearDown();
                }

                bulletRepo.TryRemove(bulletEntity);
            }
        }
        #endregion

        #region [Item]
        void Tick_ItemAssetsSpawn(int nextFrame)
        {
            if (itemSpawnQueue.TryPeek(out var itemSpawnMsg))
            {
                itemSpawnQueue.Dequeue();
                clientFrame = nextFrame;

                var itemTypeArray = itemSpawnMsg.itemTypeArray;
                var subtypeArray = itemSpawnMsg.subtypeArray;
                var entityIdArray = itemSpawnMsg.entityIdArray;
                var fieldEntity = worldFacades.Repo.FiledEntityRepo.CurFieldEntity;
                AssetPointEntity[] assetPointEntities = fieldEntity.transform.GetComponentsInChildren<AssetPointEntity>();
 
                for (int index = 0; index < assetPointEntities.Length; index++)
                {
                    Transform parent = assetPointEntities[index].transform;
                    ItemType itemType = (ItemType)itemTypeArray[index];
                    ushort entityId = entityIdArray[index];
                    byte subtype = subtypeArray[index];

                    // 生成资源
                    var itemDomain = worldFacades.Domain.ItemDomain;
                    var item = itemDomain.SpawnItem(itemType, subtype);
                    item.transform.SetParent(parent);
                    item.transform.localPosition = Vector3.zero;

                    // Entity以及Repo
                    switch (itemType)
                    {
                        case ItemType.Default:
                            break;
                        case ItemType.Weapon:
                            var weaponEntity = item.GetComponent<WeaponEntity>();
                            var weaponRepo = worldFacades.Repo.WeaponRepo;
                            weaponEntity.Ctor();
                            weaponEntity.SetEntityId(entityId);
                            weaponRepo.Add(weaponEntity);
                            break;
                        case ItemType.Bullet:
                            var bulletEntity = item.GetComponent<BulletEntity>();
                            var bulletItemRepo = worldFacades.Repo.BulletItemRepo;
                            bulletEntity.Ctor();
                            bulletEntity.MoveComponent.enable = false;
                            bulletEntity.SetEntityId(entityId);
                            bulletItemRepo.Add(bulletEntity);
                            break;
                        case ItemType.Pill:
                            break;
                    }
                }
                Debug.Log($"地图资源生成完毕******************************************************");
            }
        }

        void Tick_ItemPick(int nextFrame)
        {
            if (itemPickQueue.TryPeek(out var msg))
            {
                itemPickQueue.Dequeue();
                clientFrame = nextFrame;

                var masterWRID = msg.wRid;
                var itemType = (ItemType)msg.itemType;
                var itemEntityId = msg.entityId;

                var itemDomain = worldFacades.Domain.ItemDomain;
                var repo = worldFacades.Repo;
                var roleRepo = repo.WorldRoleRepo;
                var role = roleRepo.Get(msg.wRid);
                Debug.Log(role.roleRenderer.handPoint.name);
                bool isPickUpSucceed = itemDomain.TryPickUpItem(itemType, itemEntityId, repo, role, role.roleRenderer.handPoint);
                Debug.Log($"[wRid:{masterWRID}]拾取 {itemType.ToString()}物件[entityId:{itemEntityId}]");
            }
        }
        #endregion

        #endregion

        #region [Server Response]

        #region [ROLE] 
        void OnWRoleSync(WRoleStateUpdateMsg msg)
        {
            stateQueue.Enqueue(msg);
        }

        void OnWorldRoleSpawn(FrameWRoleSpawnResMsg msg)
        {
            // Debug.Log("加入角色生成队列");
            roleSpawnQueue.Enqueue(msg);
        }
        #endregion

        #region [BULLET]
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
        #endregion

        #region [ITEM]
        void OnItemSpawn(FrameItemSpawnResMsg msg)
        {
            Debug.Log($"加入武器生成队列");
            itemSpawnQueue.Enqueue(msg);
        }

        // ITEM
        void OnItemPickUp(FrameItemPickResMsg msg)
        {
            Debug.Log($"加入物件拾取队列");
            itemPickQueue.Enqueue(msg);
        }
        #endregion

        #endregion

        #region [Network Event Center]

        async void EnterWorldChooseScene()
        {
            // 当前有加载好的场景，则不加载
            var curFieldEntity = worldFacades.Repo.FiledEntityRepo.CurFieldEntity;
            if (curFieldEntity != null) return;

            // Load Scene And Spawn Field
            var domain = worldFacades.Domain;
            var fieldEntity = await domain.WorldSpawnDomain.SpawnCityScene();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            fieldEntity.SetFieldId(1);
            var fieldEntityRepo = worldFacades.Repo.FiledEntityRepo;
            var physicsScene = fieldEntity.gameObject.scene.GetPhysicsScene();
            fieldEntityRepo.Add(fieldEntity);
            fieldEntityRepo.SetPhysicsScene(physicsScene);
            // Send Spawn Role Message
            var rqs = worldFacades.Network.WorldRoleReqAndRes;
            rqs.SendReq_WolrdRoleSpawn(clientFrame);
        }

        #endregion

        #region [Private Func]

        bool WillHitOtherRole(WorldRoleLogicEntity roleEntity, Vector3 moveDir)
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

        #endregion

    }

}
using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness;
using Game.Server.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.WorldBussiness.Generic;
using Game.Server.Bussiness.Center;

namespace Game.Server.Bussiness.WorldBussiness
{

    public class WorldPhysicsController
    {

        WorldFacades worldFacades;
        float fixedDeltaTime;

        // 当前所有ConnId
        List<int> connIdList => worldFacades.Network.connIdList;

        public WorldPhysicsController()
        {

        }

        public void Inject(WorldFacades worldFacades, float fixedDeltaTime)
        {
            this.worldFacades = worldFacades;
            this.fixedDeltaTime = fixedDeltaTime;
        }

        public void Tick()
        {
            if (!EventCenter.stopPhyscisOneFrame)
            {
                // Physics Simulation
                Tick_Physics_Movement_Bullet(fixedDeltaTime);
                Tick_Physics_Movement_Role(fixedDeltaTime);
                var physicsScene = worldFacades.ClientWorldFacades.Repo.FiledRepo.CurPhysicsScene;
                physicsScene.Simulate(fixedDeltaTime);
            }
            else
            {
                EventCenter.stopPhyscisOneFrame = false;
            }

            // Physcis Collision
            Tick_Physics_Collision_Role();
            Tick_Physics_Collision_Bullet();
        }

        #region [Physics]

        // 地形造成的减速 TODO:滑铲加速
        void Tick_Physics_Collision_Role()
        {
            var physicsDomain = worldFacades.ClientWorldFacades.Domain.PhysicsDomain;
            var roleList = physicsDomain.Tick_AllRoleHitEnter(fixedDeltaTime);
            var rqs = worldFacades.Network.WorldRoleReqAndRes;
            roleList.ForEach((role) =>
            {
                connIdList.ForEach((connId) =>
                {
                    rqs.SendUpdate_WRoleState(connId, role);
                });
            });

        }

        void Tick_Physics_Collision_Bullet()
        {
            var physicsDomain = worldFacades.ClientWorldFacades.Domain.PhysicsDomain;
            physicsDomain.Refresh_BulletHit();

            var bulletDomain = worldFacades.ClientWorldFacades.Domain.BulletDomain;
            var bulletRepo = worldFacades.ClientWorldFacades.Repo.BulletRepo;
            List<BulletEntity> removeList = new List<BulletEntity>();
            bulletRepo.Foreach((bullet) =>
            {
                bool isHitSomething = false;
                if (bullet.HitRoleQueue.TryDequeue(out var wrole))
                {
                    isHitSomething = true;
                    // Server Logic
                    wrole.HealthComponent.HurtByBullet(bullet);
                    wrole.MoveComponent.HitByBullet(bullet);
                    if (wrole.HealthComponent.IsDead)
                    {
                        wrole.TearDown();
                        wrole.Reborn();
                    }
                    // Notice Client
                    var rqs = worldFacades.Network.BulletReqAndRes;
                    connIdList.ForEach((connId) =>
                    {
                        rqs.SendRes_BulletHitRole(connId, bullet.EntityId, wrole.EntityId);
                    });

                }
                if (bullet.HitFieldQueue.TryDequeue(out var field))
                {
                    isHitSomething = true;
                    // TODO:Server Logic
                    // Notice Client
                    var rqs = worldFacades.Network.BulletReqAndRes;
                    connIdList.ForEach((connId) =>
                    {
                        rqs.SendRes_BulletHitWall(connId, bullet);
                    });
                }

                if (isHitSomething)
                {
                    // Server Logic
                    if (bullet.BulletType == BulletType.DefaultBullet)
                    {
                        // 普通子弹的逻辑，只是单纯的移除
                        removeList.Add(bullet);
                    }
                    if (bullet is HookerEntity hookerEntity)
                    {
                        // 爪钩逻辑
                        if (field != null) hookerEntity.TryGrabSomthing(field.transform);
                        if (wrole != null) hookerEntity.TryGrabSomthing(wrole.transform);
                    }
                    else if (bullet is GrenadeEntity grenadeEntity)
                    {
                        // 手雷逻辑: 速度清零
                        grenadeEntity.MoveComponent.SetMoveVelocity(Vector3.zero);
                    }

                }
            });
            removeList.ForEach((bullet) =>
            {
                bullet.TearDown();
                bulletRepo.TryRemove(bullet);
            });
        }

        void Tick_Physics_Movement_Role(float fixedDeltaTime)
        {
            var domain = worldFacades.ClientWorldFacades.Domain.WorldRoleDomain;
            domain.Tick_RoleRigidbody(fixedDeltaTime);
        }

        void Tick_Physics_Movement_Bullet(float fixedDeltaTime)
        {
            var domain = worldFacades.ClientWorldFacades.Domain.BulletDomain;
            domain.Tick_Bullet(fixedDeltaTime);
        }

        #endregion


    }

}
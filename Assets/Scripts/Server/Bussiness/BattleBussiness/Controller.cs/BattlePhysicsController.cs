using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattlePhysicsController
    {

        BattleFacades battleFacades;
        float fixedDeltaTime;

        // 当前所有ConnId
        List<int> connIdList => battleFacades.Network.connIdList;

        public BattlePhysicsController()
        {

        }

        public void Inject(BattleFacades battleFacades, float fixedDeltaTime)
        {
            this.battleFacades = battleFacades;
            this.fixedDeltaTime = fixedDeltaTime;
        }

        public void Tick()
        {
            // Physics Simulation
            Tick_Physics_Movement_Bullet(fixedDeltaTime);
            Tick_Physics_Movement_Role(fixedDeltaTime);
            var physicsScene = battleFacades.ClientBattleFacades.Repo.FiledRepo.CurPhysicsScene;
            physicsScene.Simulate(fixedDeltaTime);

            // Physcis Collision
            Tick_Physics_Collision_Role();
            Tick_Physics_Collision_Bullet();
        }

        #region [Physics]

        // 地形造成的减速 TODO:滑铲加速
        void Tick_Physics_Collision_Role()
        {
            var physicsDomain = battleFacades.ClientBattleFacades.Domain.PhysicsDomain;
            var roleList = physicsDomain.Tick_AllRoleHitEnter(fixedDeltaTime);
            var rqs = battleFacades.Network.RoleReqAndRes;
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
            var physicsDomain = battleFacades.ClientBattleFacades.Domain.PhysicsDomain;
            var bulletDomain = battleFacades.ClientBattleFacades.Domain.BulletDomain;
            var bulletRepo = battleFacades.ClientBattleFacades.Repo.BulletRepo;
            List<BulletEntity> removeList = new List<BulletEntity>();

            bulletRepo.Foreach((bullet) =>
            {
                var hitRoleList = physicsDomain.GetHitRole_ColliderList(bullet);
                Transform hookedRoleTrans = null;

                hitRoleList.ForEach((ce) =>
                {
                    // TODO:如果子弹不能穿透，则是直接退出循环

                    if (hookedRoleTrans == null)
                    {
                        hookedRoleTrans = ce.gameObject.transform;
                    }

                    var role = ce.gameObject.GetComponentInParent<BattleRoleLogicEntity>();
                    if (bullet.MasterId != role.IDComponent.EntityId)
                    {
                        Debug.Log($"打中敌人");

                        // TODO: 配置表配置 ----------------------------------------------
                        HitPowerModel hitPowerModel = new HitPowerModel();
                        hitPowerModel.damage = 5;
                        hitPowerModel.canHitRepeatly = false;
                        hitPowerModel.attackTag = AttackTag.Enemy;
                        // ----------------------------------------------------------------

                        var service = battleFacades.ClientBattleFacades.ArbitrationService;
                        if (service.CanDamage(role.IDComponent, bullet.IDComponent, in hitPowerModel))
                        {
                            role.HealthComponent.HurtByDamage(hitPowerModel.damage);
                            var roleRqs = battleFacades.Network.RoleReqAndRes;

                            role.MoveComponent.HitByBullet(bullet);
                            if (role.HealthComponent.IsDead())
                            {
                                role.TearDown();
                                var roleDomain = battleFacades.ClientBattleFacades.Domain.RoleDomain;
                                roleDomain.RebornRole(role);
                            }

                            // 广播 1
                            connIdList.ForEach((connID) =>
                            {
                                roleRqs.SendUpdate_WRoleState(connID, role);
                            });

                        }

                        // Notice Client
                        var rqs = battleFacades.Network.BulletReqAndRes;
                        connIdList.ForEach((connId) =>
                        {
                            // 广播 2
                            rqs.SendRes_BulletHitRole(connId, bullet.IDComponent.EntityId, role.IDComponent.EntityId);
                        });
                    }

                });

                var hitFieldList = physicsDomain.GetHitField_ColliderList(bullet);
                Transform field = null;
                hitFieldList.ForEach((ce) =>
                {
                    // Notice Client
                    var rqs = battleFacades.Network.BulletReqAndRes;
                    connIdList.ForEach((connId) =>
                    {
                        rqs.SendRes_BulletHitField(connId, bullet);
                    });
                    field = ce.gameObject.transform;
                });

                if (hitRoleList.Count != 0 || hitFieldList.Count != 0)
                {
                    // 普通子弹的逻辑，只是单纯的TearDown
                    if (bullet.BulletType == BulletType.DefaultBullet)
                    {
                        removeList.Add(bullet);
                    }

                    // 爪钩逻辑
                    if (bullet is HookerEntity hookerEntity)
                    {
                        hookerEntity.TryGrabSomthing(field);
                        hookerEntity.TryGrabSomthing(hookedRoleTrans);
                    }
                    // 手雷逻辑: 速度清零
                    else if (bullet is GrenadeEntity grenadeEntity)
                    {
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
            var physicsDomain = battleFacades.ClientBattleFacades.Domain.PhysicsDomain;
            physicsDomain.Tick_RoleMoveHitErase();   //Unity's Collision Will Auto Erase
            var domain = battleFacades.ClientBattleFacades.Domain.RoleDomain;
            domain.Tick_RoleRigidbody(fixedDeltaTime);
        }

        void Tick_Physics_Movement_Bullet(float fixedDeltaTime)
        {
            var domain = battleFacades.ClientBattleFacades.Domain.BulletDomain;
            domain.Tick_Bullet(fixedDeltaTime);
        }

        #endregion


    }

}
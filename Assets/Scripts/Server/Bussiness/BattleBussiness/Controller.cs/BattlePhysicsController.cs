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
            var rqs = battleFacades.Network.BattleRoleReqAndRes;
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
                Transform roleTrans = null;
                hitRoleList.ForEach((ce) =>
                {
                    roleTrans = ce.gameObject.transform;
                    // Server Logic
                    var role = ce.gameObject.GetComponentInParent<BattleRoleLogicEntity>();
                    if (bullet.MasterId != role.EntityId)
                    {
                        Debug.Log($"打中敌人 bullet.MasterId ：{bullet.MasterId} role.EntityId ：{role.EntityId} ");
                        role.HealthComponent.HurtBy(bullet);
                        role.MoveComponent.HitByBullet(bullet);
                        if (role.HealthComponent.IsDead)
                        {
                            role.TearDown();
                            role.Reborn();
                        }
                        // Notice Client
                        var rqs = battleFacades.Network.BulletReqAndRes;
                        connIdList.ForEach((connId) =>
                        {
                            rqs.SendRes_BulletHitRole(connId, bullet.EntityId, role.EntityId);
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
                    // Server Logic
                    if (bullet.BulletType == BulletType.DefaultBullet)
                    {
                        // 普通子弹的逻辑，只是单纯的移除
                        removeList.Add(bullet);
                    }

                    if (bullet is HookerEntity hookerEntity)
                    {
                        // 爪钩逻辑
                        hookerEntity.TryGrabSomthing(field);
                        hookerEntity.TryGrabSomthing(roleTrans);
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
            // var physicsDomain = battleFacades.ClientBattleFacades.Domain.PhysicsDomain;
            // physicsDomain.Tick_RoleMoveHitErase();   //Unity's Collision Will Auto Erase
            var domain = battleFacades.ClientBattleFacades.Domain.BattleRoleDomain;
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
using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;

namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class PhysicsDomain
    {
        WorldFacades worldFacades;

        public PhysicsDomain()
        {
        }

        public void Inject(WorldFacades facades)
        {
            this.worldFacades = facades;
        }

        public List<ColliderExtra> GetHitField_ColliderList(PhysicsEntity physicsEntity) => GetColliderList(physicsEntity, "Field");
        public List<ColliderExtra> GetHitRole_ColliderList(PhysicsEntity physicsEntity) => GetColliderList(physicsEntity, "Role");

        public List<WorldRoleLogicEntity> Tick_AllRoleHitEnter()
        {
            List<WorldRoleLogicEntity> roleList = new List<WorldRoleLogicEntity>();
            var roleRepo = worldFacades.Repo.WorldRoleRepo;
            roleRepo.Foreach((role) =>
            {
                var rolePos = role.SelfPos;
                // 墙体撞击：速度管理
                var wallColliderList = GetHitField_ColliderList(role);
                wallColliderList.ForEach((colliderExtra) =>
                {
                    if (colliderExtra.isEnter == CollisionStatus.Enter)
                    {
                        colliderExtra.isEnter = CollisionStatus.Stay;
                        var collider = colliderExtra.collider;
                        var closestPoint = collider.ClosestPoint(rolePos);
                        var hitDir = (closestPoint - rolePos).normalized;
                        role.MoveComponent.HitSomething(hitDir);
                        if (hitDir.y < -0.1f) role.MoveComponent.EnterGound();
                        else role.MoveComponent.EnterWall();
                        role.SetRoleState(RoleState.Normal);

                        roleList.Add(role);
                    }
                    else if (colliderExtra.isEnter == CollisionStatus.Exit)
                    {
                        var collider = colliderExtra.collider;
                        var closestPoint = collider.ClosestPoint(rolePos);
                        var leaveDir = (rolePos - closestPoint).normalized;
                        role.MoveComponent.LeaveSomthing(leaveDir);
                        if (leaveDir.y > 0.1f) role.MoveComponent.LeaveGround();
                        else role.MoveComponent.LeaveWall();

                        role.RemoveHitCollider(colliderExtra);
                    }

                });
            });

            return roleList;
        }

        public void Tick_RoleMoveHitErase(WorldRoleLogicEntity role)
        {
            var rolePos = role.SelfPos;
            // 墙体撞击：速度管理
            var wallColliderList = GetHitField_ColliderList(role);
            wallColliderList.ForEach((colliderExtra) =>
            {
                var collider = colliderExtra.collider;
                var closestPoint = collider.ClosestPoint(rolePos);
                var hitDir = (closestPoint - rolePos).normalized;
                role.MoveComponent.MoveHitErase(hitDir);
            });
        }

        public void Refresh_BulletHit()
        {
            var bulletRepo = worldFacades.Repo.BulletRepo;
            bulletRepo.Foreach((bullet) =>
            {
                var roleColliderList = GetHitRole_ColliderList(bullet);
                var hitRoleQueue = bullet.HitRoleQueue;
                roleColliderList.ForEach((colliderExtra) =>
                {
                    if (colliderExtra.isEnter == CollisionStatus.Enter)
                    {
                        colliderExtra.isEnter = CollisionStatus.Stay;
                        var role = colliderExtra.collider.GetComponent<WorldRoleLogicEntity>();
                        hitRoleQueue.Enqueue(role);
                    }
                });

                var fieldColliderList = GetHitField_ColliderList(bullet);
                var hitWallQueue = bullet.HitFieldQueue;
                fieldColliderList.ForEach((colliderExtra) =>
                {
                    if (colliderExtra.isEnter == CollisionStatus.Enter)
                    {
                        colliderExtra.isEnter = CollisionStatus.Stay;
                        var field = colliderExtra.collider.gameObject;
                        hitWallQueue.Enqueue(field);
                    }
                });

            });
        }

        List<ColliderExtra> GetColliderList(PhysicsEntity physicsEntity, string layerName)
        {
            List<ColliderExtra> colliderList = new List<ColliderExtra>();
            List<ColliderExtra> removeList = new List<ColliderExtra>();
            physicsEntity.HitColliderListForeach((colliderExtra) =>
            {
                if (colliderExtra.collider == null)
                {
                    removeList.Add(colliderExtra);
                    return;
                }

                var name = LayerMask.LayerToName(colliderExtra.collider.gameObject.layer);
                //= 墙体
                if (name == layerName)
                {
                    colliderList.Add(colliderExtra);
                }
            });

            removeList.ForEach((ce) =>
            {
                physicsEntity.RemoveHitCollider(ce);
            });

            return colliderList;
        }

    }

}
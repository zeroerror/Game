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

        public List<WorldRoleEntity> Tick_AllRoleHitEnter()
        {
            List<WorldRoleEntity> roleList = new List<WorldRoleEntity>();
            var roleRepo = worldFacades.Repo.WorldRoleRepo;
            roleRepo.Foreach((role) =>
            {
                var rolePos = role.selfPos;
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
                });
            });

            return roleList;
        }

        public void Tick_RoleMoveHitErase(WorldRoleEntity role)
        {
            var rolePos = role.selfPos;
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

        List<ColliderExtra> GetColliderList(PhysicsEntity physicsEntity, string layerName)
        {
            List<ColliderExtra> colliderList = new List<ColliderExtra>();
            physicsEntity.HitColliderListForeach((colliderExtra) =>
            {
                var name = LayerMask.LayerToName(colliderExtra.collider.gameObject.layer);
                //= 墙体
                if (name == layerName)
                {
                    colliderList.Add(colliderExtra);
                }
            });

            return colliderList;
        }


    }

}
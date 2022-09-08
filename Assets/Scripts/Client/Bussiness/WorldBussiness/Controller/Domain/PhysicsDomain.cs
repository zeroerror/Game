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

        public List<Collider> GetHitWall_ColliderList(PhysicsEntity physicsEntity) => GetColliderList(physicsEntity, "Field");
        public List<Collider> GetHitRole_ColliderList(PhysicsEntity physicsEntity) => GetColliderList(physicsEntity, "Role");

        public void Tick_RoleHit()
        {
            var roleRepo = worldFacades.Repo.WorldRoleRepo;
            roleRepo.Foreach((role) =>
            {
                var rolePos= role.selfPos;
                // 墙体撞击：速度管理
                var wallColliderList = GetHitWall_ColliderList(role);
                wallColliderList.ForEach((wall) =>
                {
                    var wallPos = wall.transform.position;  
                });

                if (role.RoleState != RoleState.Hooking)
                {
                    role.SetRoleState(RoleState.Normal);
                    role.MoveComponent.EnterField();
                    role.AnimatorComponent.PlayIdle();
                }
            });
        }


        List<Collider> GetColliderList(PhysicsEntity physicsEntity, string layerName)
        {
            List<Collider> colliderList = new List<Collider>();
            physicsEntity.HitColliderListForeach((collider) =>
            {
                var name = LayerMask.LayerToName(collider.gameObject.layer);
                //= 墙体
                if (name == layerName)
                {
                    colliderList.Add(collider);
                }
            });

            return colliderList;
        }


    }

}
using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class PhysicsDomain
    {
        BattleFacades battleFacades;

        public PhysicsDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public List<ColliderExtra> GetHitField_ColliderList(PhysicsEntity physicsEntity) => GetColliderList(physicsEntity, "Field");
        public List<ColliderExtra> GetHitItem_ColliderList(PhysicsEntity physicsEntity) => GetColliderList(physicsEntity, "Item");
        public List<ColliderExtra> GetHitRole_ColliderList(PhysicsEntity physicsEntity) => GetColliderList(physicsEntity, "Role");

        public List<BattleRoleLogicEntity> Tick_AllRoleHitEnter(float fixedDeltaTime)
        {
            List<BattleRoleLogicEntity> hitRoleList = new List<BattleRoleLogicEntity>();
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                var rolePos = role.SelfPos;
                // 墙体撞击的速度管理
                var wallColliderList = GetHitField_ColliderList(role);
                int enterGroundCount = 0;
                int hitWallCount = 0;
                wallColliderList.ForEach((colliderExtra) =>
                {
                    var collider = colliderExtra.collider;
                    var closestPoint = collider.ClosestPoint(rolePos);
                    var hitDir = (closestPoint - rolePos).normalized;
                    role.MoveComponent.HitSomething(hitDir);

                    if (colliderExtra.isEnter != CollisionStatus.Exit)
                    {
                        if (hitDir.y < 0) enterGroundCount++;
                        else hitWallCount++;
                    }

                    if (colliderExtra.isEnter == CollisionStatus.Enter)
                    {
                        colliderExtra.isEnter = CollisionStatus.Stay;
                        if (collider.gameObject.tag == "Jumpboard")
                        {
                            role.MoveComponent.JumpboardSpeedUp();
                        }
                        hitRoleList.Add(role);
                    }
                    else if (colliderExtra.isEnter == CollisionStatus.Stay)
                    {

                    }
                    else if (colliderExtra.isEnter == CollisionStatus.Exit)
                    {
                        var leaveDir = -hitDir;
                        role.MoveComponent.LeaveSomthing(leaveDir);
                        if (leaveDir.y < 0f) hitWallCount--;
                        else enterGroundCount--;
                        role.RemoveHitCollider(colliderExtra);
                    }

                });

                if (enterGroundCount <= 0) role.MoveComponent.LeaveGround();
                else
                {
                    role.MoveComponent.EnterGound();
                }

                if (hitWallCount <= 0) role.MoveComponent.LeaveWall();
                else role.MoveComponent.EnterWall();

            });

            return hitRoleList;
        }

        public void Tick_RoleMoveHitErase()
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
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
            });
        }

        public void Refresh_BulletHit()
        {
            var bulletRepo = battleFacades.Repo.BulletRepo;
            bulletRepo.Foreach((bullet) =>
            {
                var roleColliderList = GetHitRole_ColliderList(bullet);
                var hitRoleQueue = bullet.HitRoleQueue;
                roleColliderList.ForEach((colliderExtra) =>
                {
                    if (colliderExtra.isEnter == CollisionStatus.Enter)
                    {
                        colliderExtra.isEnter = CollisionStatus.Stay;
                        var role = colliderExtra.collider.GetComponent<BattleRoleLogicEntity>();
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
                if (colliderExtra.collider == null || colliderExtra.collider.enabled == false || colliderExtra.isEnter == CollisionStatus.Exit)
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
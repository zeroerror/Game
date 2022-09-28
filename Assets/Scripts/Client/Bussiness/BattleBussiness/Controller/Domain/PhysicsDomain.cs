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

        public List<CollisionExtra> GetHitField_ColliderList(PhysicsEntity physicsEntity) => GetCollisionExtraList(physicsEntity, "Field");
        public List<CollisionExtra> GetHitItem_ColliderList(PhysicsEntity physicsEntity) => GetCollisionExtraList(physicsEntity, "Item");
        public List<CollisionExtra> GetHitRole_ColliderList(PhysicsEntity physicsEntity) => GetCollisionExtraList(physicsEntity, "Role");

        public List<BattleRoleLogicEntity> Tick_AllRoleHitEnter(float fixedDeltaTime)
        {
            List<BattleRoleLogicEntity> hitRoleList = new List<BattleRoleLogicEntity>();
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                var rolePos = role.SelfPos;
                // 墙体撞击的速度管理
                var fieldColliderList = GetHitField_ColliderList(role);
                int enterGroundCount = 0;
                int hitWallCount = 0;
                fieldColliderList.ForEach((colliderExtra) =>
                {
                    var collision = colliderExtra.collision;
                    var closestPoint = collision.collider.bounds.ClosestPoint(rolePos);
                    bool hasContact = true;
                    if (closestPoint == rolePos)
                    {
                        if (collision.contactCount != 0)
                        {
                            closestPoint = collision.GetContact(0).point;
                            colliderExtra.lastContactPoint = closestPoint;
                        }
                        else
                        {
                            closestPoint = colliderExtra.lastContactPoint;
                            hasContact = false;
                        }
                    }
                    var hitDir = (closestPoint - rolePos).normalized;
                    if (!hasContact) hitDir = -hitDir;
                    role.MoveComponent.HitSomething(hitDir);
                    if (colliderExtra.isEnter != CollisionStatus.Exit)
                    {
                        if (hitDir.y < 0) enterGroundCount++;
                        else hitWallCount++;
                    }

                    if (colliderExtra.isEnter == CollisionStatus.Enter)
                    {
                        colliderExtra.isEnter = CollisionStatus.Stay;
                        if (collision.gameObject.tag == "Jumpboard")
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
                    }

                });

                if (enterGroundCount <= 0) role.MoveComponent.LeaveGround();
                else
                {
                    role.MoveComponent.EnterGound();
                    role.SetRoleState(RoleState.Normal);
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
                var fieldColliderList = GetHitField_ColliderList(role);
                fieldColliderList.ForEach((colliderExtra) =>
                {
                    var collision = colliderExtra.collision;
                    var closestPoint = collision.collider.bounds.ClosestPoint(rolePos);
                    if (closestPoint == rolePos)
                    {
                        if (collision.contactCount != 0)
                        {
                            closestPoint = collision.GetContact(0).point;
                            colliderExtra.lastContactPoint = closestPoint;
                        }
                        else
                        {
                            closestPoint = colliderExtra.lastContactPoint;
                        }
                    }
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
                        var role = colliderExtra.colliderForTrigger.GetComponent<BattleRoleLogicEntity>();
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
                        var field = colliderExtra.Collider.gameObject;
                        hitWallQueue.Enqueue(field);
                    }
                });

            });
        }

        List<CollisionExtra> GetCollisionExtraList(PhysicsEntity physicsEntity, string layerName)
        {
            List<CollisionExtra> collisionList = new List<CollisionExtra>();
            List<CollisionExtra> removeList = new List<CollisionExtra>();
            physicsEntity.HitCollisionExtraListForeach((System.Action<CollisionExtra>)((collisionExtra) =>
            {
                string name = string.Empty;

                // Collider(Has Collision Info)
                if (collisionExtra.collision != null)
                {
                    if (collisionExtra.collision.collider == null || collisionExtra.collision.collider.enabled == false || collisionExtra.isEnter == CollisionStatus.Exit)
                    {
                        removeList.Add(collisionExtra);
                        return;
                    }

                    name = LayerMask.LayerToName(collisionExtra.collision.gameObject.layer);
                    if (name == layerName) collisionList.Add((CollisionExtra)collisionExtra);
                    return;
                }

                // Trigger
                if (collisionExtra.colliderForTrigger == null || collisionExtra.colliderForTrigger.enabled == false || collisionExtra.isEnter == CollisionStatus.Exit)
                {
                    removeList.Add(collisionExtra);
                    return;
                }
                if (collisionExtra.colliderForTrigger != null)
                {
                    name = LayerMask.LayerToName(collisionExtra.colliderForTrigger.gameObject.layer);
                    if (name == layerName) collisionList.Add((CollisionExtra)collisionExtra);
                    return;
                }

            }));

            removeList.ForEach((ce) =>
            {
                physicsEntity.RemoveHitCollisionExtra(ce);
            });

            return collisionList;
        }

    }

}
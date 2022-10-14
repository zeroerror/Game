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
                fieldColliderList.ForEach((collisionExtra) =>
                {
                    var collision = collisionExtra.collision;
                    var closestPoint = collision.collider.bounds.ClosestPoint(rolePos);
                    var hitDir = collisionExtra.hitDir;
                    role.MoveComponent.HitSomething(hitDir);
                    if (collisionExtra.status != CollisionStatus.Exit)
                    {
                        if (collisionExtra.fieldType == FieldType.Ground) enterGroundCount++;
                        else if (collisionExtra.fieldType == FieldType.Wall) hitWallCount++;
                    }

                    if (collisionExtra.status == CollisionStatus.Enter)
                    {
                        collisionExtra.status = CollisionStatus.Stay;
                        if (collision.gameObject.tag == "Jumpboard")
                        {
                            role.MoveComponent.JumpboardSpeedUp();
                        }
                        hitRoleList.Add(role);
                    }
                    else if (collisionExtra.status == CollisionStatus.Stay)
                    {

                    }
                    else if (collisionExtra.status == CollisionStatus.Exit)
                    {
                        var leaveDir = -hitDir;
                        role.MoveComponent.LeaveSomthing(leaveDir);
                        if (collisionExtra.fieldType == FieldType.Wall) hitWallCount--;
                        else if (collisionExtra.fieldType == FieldType.Ground) enterGroundCount--;
                    }

                });

                // 人物撞击状态管理
                if (enterGroundCount <= 0)
                {
                    role.MoveComponent.LeaveGround();
                }
                else
                {
                    role.MoveComponent.EnterGound();
                    role.SetRoleState(RoleState.Normal);
                }

                if (hitWallCount <= 0)
                {
                    role.MoveComponent.LeaveWall();
                }
                else
                {
                    role.MoveComponent.EnterWall();
                }

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
                    role.MoveComponent.MoveHitErase(colliderExtra.hitDir);
                });
            });
        }

        List<CollisionExtra> GetCollisionExtraList(PhysicsEntity physicsEntity, string layerName)
        {
            List<CollisionExtra> collisionList = new List<CollisionExtra>();
            List<CollisionExtra> removeList = new List<CollisionExtra>();
            physicsEntity.HitCollisionExtraListForeach((collisionExtra) =>
            {

                if (collisionExtra.status == CollisionStatus.Exit) removeList.Add(collisionExtra);
                Collider collider = collisionExtra.Collider;
                if (collider == null || collider.enabled == false)
                {
                    // 目标被摧毁,等价于Exit
                    collisionExtra.status = CollisionStatus.Exit;
                    removeList.Add(collisionExtra);
                }

                if (collisionExtra.layerName == layerName) collisionList.Add(collisionExtra);   //本帧依然添加进List
            });

            removeList.ForEach((ce) =>
            {
                physicsEntity.RemoveHitCollisionExtra(ce);
            });

            return collisionList;
        }

    }

}
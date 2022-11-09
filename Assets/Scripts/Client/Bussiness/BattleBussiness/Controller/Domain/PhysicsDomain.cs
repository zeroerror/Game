using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

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
        public List<CollisionExtra> GetHitAirdrop_ColliderList(PhysicsEntity physicsEntity) => GetCollisionExtraList(physicsEntity, "Airdrop");
        public List<CollisionExtra> GetHitRole_ColliderList(PhysicsEntity physicsEntity) => GetCollisionExtraList(physicsEntity, "Role");

        public void Tick_Physics_Collections_Role_Field()
        {
            var roleRepo = battleFacades.Repo.RoleLogicRepo;
            roleRepo.Foreach((role) =>
            {
                PhysicsEntityHitField(role, role.LocomotionComponent);
            });
        }

        public void Tick_Physics_Collections_Bullet_Field()
        {
            Transform hitTrans = null;
            var bulletRepo = battleFacades.Repo.BulletLogicRepo;
            var bulletDomain = battleFacades.Domain.BulletLogicDomain;

            bulletRepo.ForAll((bullet) =>
            {
                var hitFieldList = GetHitField_ColliderList(bullet);
                hitFieldList.ForEach((ce) =>
                {
                    if (ce.status != CollisionStatus.Enter)
                    {
                        return;
                    }
                    ce.status = CollisionStatus.Stay;

                    HitFieldModel hitFieldModel = new HitFieldModel();
                    hitFieldModel.hitter = bullet.IDComponent;
                    hitFieldModel.fieldCE = ce;

                    hitTrans = ce.GetCollider().transform;
                    // - Logic Trigger
                    var logicTriggerAPI = battleFacades.LogicTriggerEvent;
                    logicTriggerAPI.Invoke_BulletHitFieldAction(bullet.IDComponent.EntityID, hitTrans);
                });

            });
        }

        public void Tick_Physics_Collections_Airdrop_Field()
        {
            var airdropRepo = battleFacades.Repo.AirdropLogicRepo;
            airdropRepo.Foreach((airdrop) =>
            {
                PhysicsEntityHitField(airdrop, airdrop.LocomotionComponent);
            });
        }

        void PhysicsEntityHitField(PhysicsEntity entity, LocomotionComponent locomotionComponent)
        {
            var entityPos = entity.Position;

            // 墙体撞击的速度管理
            var fieldColliderList = GetHitField_ColliderList(entity);
            int enterGroundCount = 0;
            int hitWallCount = 0;
            fieldColliderList.ForEach((collisionExtra) =>
            {
                var go = collisionExtra.gameObject;
                var hitDir = collisionExtra.hitDir;
                if (collisionExtra.status != CollisionStatus.Exit)
                {
                    if (collisionExtra.fieldType == FieldType.Ground) enterGroundCount++;
                    else if (collisionExtra.fieldType == FieldType.Wall) hitWallCount++;
                }

                if (collisionExtra.status == CollisionStatus.Enter)
                {
                    collisionExtra.status = CollisionStatus.Stay;
                    if (go.tag == "Jumpboard")
                    {
                        locomotionComponent.JumpboardSpeedUp();
                    }
                }
                else if (collisionExtra.status == CollisionStatus.Stay)
                {

                }
                else if (collisionExtra.status == CollisionStatus.Exit)
                {
                    var leaveDir = -hitDir;
                    if (collisionExtra.fieldType == FieldType.Wall) hitWallCount--;
                    else if (collisionExtra.fieldType == FieldType.Ground) enterGroundCount--;
                }

            });

            // 撞击状态管理
            if (enterGroundCount <= 0)
            {
                locomotionComponent.LeaveGround();
            }
            else
            {
                locomotionComponent.EnterGound();
            }

            if (hitWallCount <= 0)
            {
                locomotionComponent.LeaveWall();
            }
            else
            {
                locomotionComponent.EnterWall();
            }
        }

        List<CollisionExtra> GetCollisionExtraList(PhysicsEntity physicsEntity, string layerName)
        {
            List<CollisionExtra> collisionList = new List<CollisionExtra>();
            List<CollisionExtra> removeList = new List<CollisionExtra>();
            physicsEntity.HitCollisionExtraListForeach((collisionExtra) =>
            {

                if (collisionExtra.status == CollisionStatus.Exit)
                {
                    removeList.Add(collisionExtra);
                }

                Collider collider = collisionExtra.GetCollider();
                if (collider == null || collider.enabled == false)
                {
                    // 目标被摧毁,等价于Exit
                    collisionExtra.status = CollisionStatus.Exit;
                    removeList.Add(collisionExtra);
                }

                if (collisionExtra.layerName == layerName)
                {
                    collisionList.Add(collisionExtra);   //本帧依然添加进List
                }
            });

            removeList.ForEach((ce) =>
            {
                physicsEntity.RemoveHitCollisionExtra(ce);
            });

            return collisionList;
        }

    }

}
using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattlePhysicsController
    {

        BattleServerFacades battleFacades;

        List<int> ConnIDList => battleFacades.Network.connIdList;

        public BattlePhysicsController()
        {
        }

        public void Inject(BattleServerFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick(float fixedDeltaTime)
        {
            // Physics Simulation
            Tick_Physics_Movement_Bullet(fixedDeltaTime);
            Tick_Physics_Movement_Role(fixedDeltaTime);
            var physicsScene = battleFacades.BattleFacades.Repo.FiledRepo.CurPhysicsScene;
            physicsScene.Simulate(fixedDeltaTime);

            // Physcis Collision
            Tick_Physics_Collision_Role();
            Tick_Physics_Collision_Bullet();
        }

        #region [Physics]

        void Tick_Physics_Collision_Role()
        {
            var physicsDomain = battleFacades.BattleFacades.Domain.PhysicsDomain;
            physicsDomain.Tick_RoleHitField();
        }

        void Tick_Physics_Collision_Bullet()
        {
            var physicsDomain = battleFacades.BattleFacades.Domain.PhysicsDomain;
            var hitFieldList = physicsDomain.Tick_BulletHitField();

            // - Send To Client
            SendBulletHitField(hitFieldList);
        }

        void Tick_Physics_Movement_Role(float fixedDeltaTime)
        {
            var physicsDomain = battleFacades.BattleFacades.Domain.PhysicsDomain;
            // physicsDomain.Tick_RoleMoveHitErase();   //Unity's Collision Will Auto Erase 有BUG!!!!!!!!
            var domain = battleFacades.BattleFacades.Domain.RoleDomain;
            domain.Tick_RoleRigidbody(fixedDeltaTime);
        }

        void Tick_Physics_Movement_Bullet(float fixedDeltaTime)
        {
            var domain = battleFacades.BattleFacades.Domain.BulletLogicDomain;
            domain.Tick_BulletMovement(fixedDeltaTime);
        }

        #endregion

        void SendBulletHitField(List<HitFieldModel> hitFieldList)
        {
            var bulletRepo = battleFacades.BattleFacades.Repo.BulletRepo;
            var bulletRqs = battleFacades.Network.BulletReqAndRes;
            hitFieldList.ForEach((hitFieldModel) =>
            {
                var bulletIDC = hitFieldModel.hitter;
                var bullet = bulletRepo.Get(bulletIDC.EntityID);

                ConnIDList.ForEach((connId) =>
                {
                    bulletRqs.SendRes_BulletHitField(connId, bullet);
                });
            });
        }

    }

}
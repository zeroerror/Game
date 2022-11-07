using System.Collections.Generic;
using Game.Client.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattlePhysicsController
    {

        BattleServerFacades serverFacades;

        List<int> ConnIDList => serverFacades.Network.connIdList;

        public BattlePhysicsController() { }

        public void Inject(BattleServerFacades facades)
        {
            serverFacades = facades;
        }

        public void Tick(float fixedDeltaTime)
        {
            // Physics Simulation
            Tick_Physics_AllPhysicsEntity(fixedDeltaTime);
            var physicsScene = serverFacades.BattleFacades.Repo.FieldRepo.CurPhysicsScene;
            physicsScene.Simulate(fixedDeltaTime);

            // Physcis Collision
            Tick_Physics_AllCollisions();
        }

        void Tick_Physics_AllPhysicsEntity(float fixedDeltaTime)
        {
            var battleFacades = serverFacades.BattleFacades;
            var roleDomain = battleFacades.Domain.RoleDomain;
            roleDomain.Tick_Physics_AllRoles(fixedDeltaTime);

            var bulletLogicDomain = battleFacades.Domain.BulletLogicDomain;
            bulletLogicDomain.Tick_Physics_AllBullets(fixedDeltaTime);
        }

        void Tick_Physics_AllCollisions()
        {
            // - Role Field
            var physicsDomain = serverFacades.BattleFacades.Domain.PhysicsDomain;
            physicsDomain.Tick_Physics_Collections_Role_Field();

            // - Bullet Field
            var hitFieldList = physicsDomain.Tick_Physics_Collections_Bullet_Field();
            SendBulletHitFieldRes(hitFieldList);
        }

        void SendBulletHitFieldRes(List<HitFieldModel> hitFieldList)
        {
            var bulletRepo = serverFacades.BattleFacades.Repo.BulletRepo;
            var bulletRqs = serverFacades.Network.BulletReqAndRes;
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
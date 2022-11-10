using System.Collections.Generic;
using UnityEngine;
using Game.Infrastructure.Generic;
using Game.Client.Bussiness.BattleBussiness;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleBulletController
    {

        BattleServerFacades serverFacades;

        List<int> ConnIDList => serverFacades.Network.connIdList;

        public BattleBulletController() { }

        public void Inject(BattleServerFacades facades)
        {
            serverFacades = facades;

            var battleFacades = serverFacades.BattleFacades;
            var logicEventCenter = battleFacades.LogicEventCenter;
            logicEventCenter.Regist_BulletHitFieldAction(LogicEventCenter_BulletHitField);
        }

        public void Tick(float fixedDeltaTime)
        {
            Tick_BulletLifeCycle(fixedDeltaTime);
        }

        void Tick_BulletLifeCycle(float fixedDeltaTime)
        {
            Tick_BulletHitEntity(fixedDeltaTime);
            Tick_BulletLifeTime(fixedDeltaTime);
        }

        void Tick_BulletLifeTime(float fixedDeltaTime)
        {
            var bulletDomain = serverFacades.BattleFacades.Domain.BulletLogicDomain;
            var lifeOverList = bulletDomain.Tick_LifeTime_All(NetworkConfig.FIXED_DELTA_TIME);

            lifeOverList.ForEach((bullet) =>
            {

                var bulletType = bullet.BulletType;

                if (bulletType == BulletType.DefaultBullet)
                {
                    bullet.TearDown();
                }

                if (bullet is GrenadeEntity grenadeEntity)
                {
                    var bulletDomain = serverFacades.BattleFacades.Domain.BulletLogicDomain;
                    bulletDomain.GrenadeExplodeTearDown(grenadeEntity);
                }

                if (bullet is HookerEntity hookerEntity)
                {
                    hookerEntity.TearDown();
                }

                var bulletRepo = serverFacades.BattleFacades.Repo.BulletLogicRepo;
                bulletRepo.TryRemove(bullet);

                var bulletRqs = serverFacades.Network.BattleReqAndRes;
                ConnIDList.ForEach((connId) =>
                {
                    var entityType = EntityType.Bullet;
                    int entityID = bullet.IDComponent.EntityID;
                    Vector3 pos = bullet.LocomotionComponent.Position;
                    bulletRqs.SendRes_EntityTearDown(connId, entityType, entityID, pos);
                });

            });
        }

        void Tick_BulletHitEntity(float fixedDeltaTime)
        {
            var bulletDomain = serverFacades.BattleFacades.Domain.BulletLogicDomain;
            var bulletRqs = serverFacades.Network.BulletReqAndRes;
            var hitRoleList = bulletDomain.Tick_HitModels_All(EntityType.BattleRole, fixedDeltaTime);
            var hitAirdropList = bulletDomain.Tick_HitModels_All(EntityType.Aridrop, fixedDeltaTime);
            ConnIDList.ForEach((connId) =>
            {
                hitRoleList.ForEach((hitModel) =>
                {
                    var victimIDC = hitModel.victimIDC;
                    var attackerIDC = hitModel.attackerIDC;
                    bulletRqs.SendRes_BulletHitEntity(connId, attackerIDC.EntityID, victimIDC.EntityID, victimIDC.EntityType);
                });
                hitAirdropList.ForEach((hitModel) =>
                {
                    var victimIDC = hitModel.victimIDC;
                    var attackerIDC = hitModel.attackerIDC;
                    bulletRqs.SendRes_BulletHitEntity(connId, attackerIDC.EntityID, victimIDC.EntityID, victimIDC.EntityType);
                });
            });
        }

        void LogicEventCenter_BulletHitField(int bulletID, Transform hitTF)
        {
            var battleFacades = serverFacades.BattleFacades;
            var bulletLogicDomain = battleFacades.Domain.BulletLogicDomain;
            var bulletLogic = battleFacades.Repo.BulletLogicRepo.Get(bulletID);
            bulletLogicDomain.ApplyEffector_BulletHitField(bulletLogic, hitTF);

            var bulletRepo = battleFacades.Repo.BulletLogicRepo;
            var bulletRqs = serverFacades.Network.BulletReqAndRes;

            ConnIDList.ForEach((connId) =>
            {
                bulletRqs.SendRes_BulletHitField(connId, bulletLogic);
            });
        }

    }

}
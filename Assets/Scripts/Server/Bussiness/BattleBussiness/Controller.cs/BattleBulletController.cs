using System.Collections.Generic;
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
        }

        public void Tick(float fixedDeltaTime)
        {
            Tick_BulletLifeCycle(fixedDeltaTime);
        }

        void Tick_BulletLifeCycle(float fixedDeltaTime)
        {
            Tick_BulletHitRole(fixedDeltaTime);
            Tick_ActiveHookerDraging(fixedDeltaTime);
            Tick_BulletLifeTime(fixedDeltaTime);
        }

        void Tick_BulletLifeTime(float fixedDeltaTime)
        {
            var bulletDomain = serverFacades.BattleFacades.Domain.BulletLogicDomain;
            var lifeOverList = bulletDomain.Tick_BulletLifeTime(NetworkConfig.FIXED_DELTA_TIME);

            lifeOverList.ForEach((bulletEntity) =>
            {

                var bulletType = bulletEntity.BulletType;

                if (bulletType == BulletType.DefaultBullet)
                {
                    bulletEntity.TearDown();
                }

                if (bulletEntity is GrenadeEntity grenadeEntity)
                {
                    var bulletDomain = serverFacades.BattleFacades.Domain.BulletLogicDomain;
                    bulletDomain.GrenadeExplode(grenadeEntity);
                }

                if (bulletEntity is HookerEntity hookerEntity)
                {
                    hookerEntity.TearDown();
                }

                var bulletRepo = serverFacades.BattleFacades.Repo.BulletRepo;
                bulletRepo.TryRemove(bulletEntity);

                var bulletRqs = serverFacades.Network.BulletReqAndRes;
                ConnIDList.ForEach((connId) =>
                {
                    // 广播子弹销毁消息
                    bulletRqs.SendRes_BulletLifeFrameOver(connId, bulletEntity);
                });

            });
        }

        void Tick_BulletHitRole(float fixedDeltaTime)
        {
            var bulletDomain = serverFacades.BattleFacades.Domain.BulletLogicDomain;
            var bulletRqs = serverFacades.Network.BulletReqAndRes;
            var hitRoleList = bulletDomain.Tick_And_Get_BulletHitRoleHitModel(fixedDeltaTime);

            ConnIDList.ForEach((connId) =>
            {
                hitRoleList.ForEach((attackModel) =>
                {
                    bulletRqs.SendRes_BulletHitRole(connId, attackModel.attackerIDC.EntityID, attackModel.victimIDC.EntityID);
                });
            });
        }

        void Tick_ActiveHookerDraging(float fixedDeltaTime)
        {
            var bulletDomain = serverFacades.BattleFacades.Domain.BulletLogicDomain;
            bulletDomain.Tick_ActiveHookerDraging(fixedDeltaTime);
        }

    }

}
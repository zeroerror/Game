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

        ServerBattleFacades serverFacades;

        List<int> ConnIDList => serverFacades.Network.connIdList;

        public BattleBulletController() { }

        public void Inject(ServerBattleFacades facades)
        {
            serverFacades = facades;

            var battleFacades = serverFacades.BattleFacades;
            var logicEventCenter = battleFacades.LogicEventCenter;
            logicEventCenter.Regist_BulletHitFieldAction(LogicEventCenter_BulletHitField);
        }

        public void Tick(float fixedDeltaTime)
        {
            Tick_BulletHitEntity(fixedDeltaTime);
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

        void LogicEventCenter_BulletHitField(int bulletID, Vector3 hitPos, Transform hitTF)
        {
            var battleFacades = serverFacades.BattleFacades;
            var bulletLogic = battleFacades.Repo.BulletLogicRepo.Get(bulletID);
            var bulletLogicDomain = battleFacades.Domain.BulletLogicDomain;
            bulletLogicDomain.ApplyEffector_BulletHitField(bulletLogic, hitPos, hitTF);

            var bulletRepo = battleFacades.Repo.BulletLogicRepo;
            var bulletRqs = serverFacades.Network.BulletReqAndRes;

            ConnIDList.ForEach((connId) =>
            {
                bulletRqs.SendRes_BulletHitField(connId, bulletLogic);
            });
        }

    }

}
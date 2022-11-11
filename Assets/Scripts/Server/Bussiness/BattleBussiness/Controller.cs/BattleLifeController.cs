using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Infrastructure.Generic;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleLifeController
    {

        ServerBattleFacades serverFacades;
        public List<int> ConnIDList => serverFacades.Network.connIdList;

        public void Inject(ServerBattleFacades battleFacades)
        {
            this.serverFacades = battleFacades;
        }

        public void Tick(float fixedDeltaTime)
        {
            Tick_Life_Role();
            Tick_Life_Airdrop();
            Tick_LifeTime_Bullet(fixedDeltaTime);
        }

        void Tick_Life_Role()
        {
            var roleRepo = serverFacades.BattleFacades.Repo.RoleLogicRepo;
            var roleDomain = serverFacades.BattleFacades.Domain.RoleLogicDomain;
            roleRepo.Foreach((role) =>
            {
                var roleState = role.StateComponent.RoleState;
                if (roleState == RoleState.Dying || roleState == RoleState.Reborning)
                {
                    return;
                }

                var healthComponent = role.HealthComponent;
                if (healthComponent.CheckIsDead())
                {
                    roleDomain.RoleState_EnterDead(role);
                    return;
                }

            });
        }

        void Tick_Life_Airdrop()
        {
            var battleFacades = serverFacades.BattleFacades;
            var airdropRepo = battleFacades.Repo.AirdropLogicRepo;
            var airdropDomain = battleFacades.Domain.AirdropLogicDomain;
            var idService = battleFacades.IDService;
            List<BattleAirdropEntity> tearDownList = new List<BattleAirdropEntity>();
            airdropRepo.ForAll((airdrop) =>
            {
                var healthComponent = airdrop.HealthComponent;
                if (healthComponent.CheckIsDead())
                {
                    airdropDomain.TearDownLogic(airdrop);
                    tearDownList.Add(airdrop);
                    return;
                }
            });

            var battleRqs = serverFacades.Network.BattleReqAndRes;
            ConnIDList.ForEach((connID) =>
            {
                tearDownList.ForEach((airdrop) =>
                {
                    var spawnEntityType = airdrop.SpawnEntityType;
                    var spawnSubType = airdrop.SpawnSubType;
                    var spawnEntityID = idService.GetAutoIDByEntityType(spawnEntityType);
                    var airdropPos = airdrop.transform.position;

                    var commonDomain = battleFacades.Domain.CommonDomain;
                    var spawnObj = commonDomain.SpawnEntity_Logic(spawnEntityType, spawnSubType, spawnEntityID, airdropPos);

                    var airdropLogicDomain = battleFacades.Domain.AirdropLogicDomain;
                    airdropLogicDomain.TearDownLogic(airdrop);

                    var idc = airdrop.IDComponent;
                    int airdropEntityID = idc.EntityID;
                    battleRqs.SendRes_BattleAirdropTearDown(connID, airdropEntityID, spawnEntityType, spawnSubType, spawnEntityID, airdropPos);
                });
            });


        }

        void Tick_LifeTime_Bullet(float fixedDeltaTime)
        {
            var bulletLogicDomain = serverFacades.BattleFacades.Domain.BulletLogicDomain;
            var lifeOverList = bulletLogicDomain.Tick_LifeTime_All(fixedDeltaTime);

            lifeOverList.ForEach((bullet) =>
            {
                bulletLogicDomain.LifeTimeOver(bullet, bullet.LocomotionComponent.Position);

                var bulletPos = bullet.LocomotionComponent.Position;
                var bulletRqs = serverFacades.Network.BulletReqAndRes;
                ConnIDList.ForEach((connId) =>
                {
                    int entityID = bullet.IDComponent.EntityID;
                    bulletRqs.SendRes_BulletLifeTimeOver(connId, entityID, bulletPos);
                });
            });
        }

    }

}
using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleLifeController
    {

        BattleServerFacades serverFacades;

        public void Inject(BattleServerFacades battleFacades)
        {
            this.serverFacades = battleFacades;
        }

        public void Tick(float fixedDeltaTime)
        {
            Tick_Life_Role();
            Tick_Life_Airdrop();
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
            serverFacades.Network.connIdList.ForEach((connID) =>
            {
                tearDownList.ForEach((airdrop) =>
                {
                    var spawnEntityType = airdrop.SpawnEntityType;
                    var spawnSubType = airdrop.SpawnSubType;
                    var spawnEntityID = idService.GetAutoIDByEntityType(spawnEntityType);
                    var airdropPos = airdrop.transform.position;

                    var itemDomain = battleFacades.Domain.ItemDomain;
                    var spawnGo = itemDomain.SpawnItem(spawnEntityType, spawnSubType, spawnEntityID);
                    spawnGo.transform.position = airdropPos;

                    battleRqs.SendRes_EntitySpawn(connID, spawnEntityType, spawnSubType, spawnEntityID, airdropPos);

                    var idc = airdrop.IDComponent;
                    int airdropEntityID = idc.EntityID;
                    battleRqs.SendRes_EntityTearDown(connID, EntityType.Aridrop, airdropEntityID, airdropPos);
                });
            });


        }

    }

}
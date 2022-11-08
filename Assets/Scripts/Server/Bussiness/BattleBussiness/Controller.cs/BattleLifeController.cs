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
                    EntityType entityType = EntityType.Aridrop;
                    var idc = airdrop.IDComponent;
                    byte subType = idc.SubType;
                    int entityID = idc.EntityID;
                    Vector3 pos = airdrop.LocomotionComponent.Position;
                    battleRqs.SendRes_EntityTearDown(connID, entityType, entityID, pos);
                });
            });


        }

    }

}
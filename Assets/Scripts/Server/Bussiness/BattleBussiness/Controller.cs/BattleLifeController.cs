using Game.Client.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleLifeController
    {

        BattleServerFacades battleFacades;
        int serveFrame;

        public void Inject(BattleServerFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick()
        {
            var roleRepo = battleFacades.BattleFacades.Repo.RoleRepo;
            var roleDomain = battleFacades.BattleFacades.Domain.RoleDomain;
            roleRepo.Foreach((role) =>
            {
                var roleState = role.StateComponent.RoleState;
                if (roleState == RoleState.Dead || roleState == RoleState.Reborn)
                {
                    return;
                }

                var healthComponent = role.HealthComponent;
                if (healthComponent.CheckIsDead())
                {
                    roleDomain.RoleStateEnterDead(role);
                }
            });
        }

    }

}
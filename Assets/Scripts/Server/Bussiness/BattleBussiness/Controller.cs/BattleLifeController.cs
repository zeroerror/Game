using Game.Client.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleLifeController
    {

        BattleServerFacades battleFacades;

        public void Inject(BattleServerFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick(float fixedDeltaTime)
        {
            var roleRepo = battleFacades.BattleFacades.Repo.RoleRepo;
            var roleDomain = battleFacades.BattleFacades.Domain.RoleDomain;
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

    }

}
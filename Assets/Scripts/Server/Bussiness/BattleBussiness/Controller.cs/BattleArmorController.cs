using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleArmorController
    {

        BattleServerFacades battleFacades;

        public void Inject(BattleServerFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick(float fixedDeltaTime)
        {
            var roleRepo = battleFacades.BattleFacades.Repo.RoleLogicRepo;
            var roleDomain = battleFacades.BattleFacades.Domain.RoleDomain;
            roleRepo.Foreach((role) =>
            {
                var armor = role.Armor;
                if (armor == null)
                {
                    return;
                }

                var health = armor.CurHealth + 1;
                var maxHealth = armor.MaxHealth;
                health = health < maxHealth ? health : maxHealth;
                armor.SetCurHealth(health);
            });
        }

    }

}
using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleArmorController
    {

        BattleFacades battleFacades;

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick(float fixedDeltaTime)
        {
            var roleRepo = battleFacades.Repo.RoleLogicRepo;
            var roleDomain = battleFacades.Domain.RoleLogicDomain;
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
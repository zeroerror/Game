using Game.Client.Bussiness.BattleBussiness.Repo;
using Game.Client.Bussiness.Repo;

namespace Game.Client.Bussiness.BattleBussiness.Facades
{

    public class AllBattleRepo
    {

        public FiledEntityRepo FiledRepo { get; private set; }

        public BattleRoleRepo RoleLogicRepo { get; private set; }
        public BattleRoleRendererRepo RoleRendererRepo { get; private set; }

        public WeaponRepo WeaponRepo { get; private set; }
        public WeaponItemRepo WeaponItemRepo { get; private set; }

        public BattleArmorRepo ArmorRepo { get; private set; }
        public BattleArmorItemRepo ArmorItemRepo { get; private set; }

        public BattleEvolveItemRepo EvolveItemRepo { get; private set; }
        
        public BulletRepo BulletRepo { get; private set; }
        public BulletRendererRepo BulletRendererRepo { get; private set; }
        public BulletItemRepo BulletItemRepo { get; private set; }


        public AllBattleRepo()
        {
            FiledRepo = new FiledEntityRepo();

            RoleLogicRepo = new BattleRoleRepo();
            RoleRendererRepo = new BattleRoleRendererRepo();

            WeaponRepo = new WeaponRepo();

            WeaponItemRepo = new WeaponItemRepo();

            ArmorRepo = new BattleArmorRepo();
            ArmorItemRepo = new BattleArmorItemRepo();

            BulletRepo = new BulletRepo();
            BulletRendererRepo = new BulletRendererRepo();
            BulletItemRepo = new BulletItemRepo();

            EvolveItemRepo = new BattleEvolveItemRepo();
        }

    }

}
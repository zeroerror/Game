using Game.Client.Bussiness.BattleBussiness.Repo;
using Game.Client.Bussiness.Repo;

namespace Game.Client.Bussiness.BattleBussiness.Facades
{

    public class AllBattleRepo
    {

        public BattleFieldRepo FieldRepo { get; private set; }

        public BattleRoleRepo RoleLogicRepo { get; private set; }
        public BattleRoleRendererRepo RoleRendererRepo { get; private set; }

        public WeaponRepo WeaponRepo { get; private set; }
        public WeaponItemRepo WeaponItemRepo { get; private set; }

        public BattleArmorRepo ArmorRepo { get; private set; }
        public BattleArmorItemRepo ArmorItemRepo { get; private set; }

        public BattleEvolveItemRepo EvolveItemRepo { get; private set; }
        
        public BulletLogicRepo BulletLogicRepo { get; private set; }
        public BulletRendererRepo BulletRendererRepo { get; private set; }
        public BulletItemRepo BulletItemRepo { get; private set; }

        public BattleAirdropLogicRepo AirdropLogicRepo { get; private set; }

        public BattleAirdropRendererRepo AirdropRendererRepo { get; private set; }

        public AllBattleRepo()
        {
            FieldRepo = new BattleFieldRepo();

            RoleLogicRepo = new BattleRoleRepo();
            RoleRendererRepo = new BattleRoleRendererRepo();

            WeaponRepo = new WeaponRepo();

            WeaponItemRepo = new WeaponItemRepo();

            ArmorRepo = new BattleArmorRepo();
            ArmorItemRepo = new BattleArmorItemRepo();

            BulletLogicRepo = new BulletLogicRepo();
            BulletRendererRepo = new BulletRendererRepo();
            BulletItemRepo = new BulletItemRepo();

            EvolveItemRepo = new BattleEvolveItemRepo();

            AirdropLogicRepo = new BattleAirdropLogicRepo();

            AirdropRendererRepo = new BattleAirdropRendererRepo();

        }

    }

}
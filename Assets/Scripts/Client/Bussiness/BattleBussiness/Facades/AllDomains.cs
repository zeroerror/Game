using Game.Client.Bussiness.BattleBussiness.Controller.Domain;

namespace Game.Client.Bussiness.BattleBussiness.Facades
{

    public class AllDomains
    {

        public BattleSceneDomain SceneDomain { get; private set; }

        public BattleRoleDomain RoleDomain { get; private set; }
        public BattleRoleRendererDomain RoleRendererDomain { get; private set; }
        public BattleRoleStateDomain RoleStateDomain { get; private set; }
        public BattleRoleStateRendererDomain RoleStateRendererDomain { get; private set; }

        public BattleInputDomain InputDomain { get; private set; }

        public PhysicsDomain PhysicsDomain { get; private set; }

        public ItemDomain ItemDomain { get; private set; }

        public BulletDomain BulletLogicDomain { get; private set; }
        public BulletRendererDomain BulletRendererDomain { get; private set; }
        public BulletItemDomain BulletItemDomain { get; private set; }

        public WeaponDomain WeaponDomain { get; private set; }
        public WeaponItemDomain WeaponItemDomain { get; private set; }

        public BattleArmorDomain ArmorDomain { get; private set; }
        public BattleArmorItemDomain ArmorItemDomain { get; private set; }
        public BattleArmorEvolveItemDomain ArmorEvolveItemDomain { get; private set; }

        public BattleHitDomain HitDomain { get; private set; }

        public BattleGameStateDomain GameStateDomain { get; private set; }

        public BattleFieldDomain FieldDomain { get; private set; }

        public AllDomains()
        {
            SceneDomain = new BattleSceneDomain();

            RoleDomain = new BattleRoleDomain();
            RoleRendererDomain = new BattleRoleRendererDomain();
            RoleStateDomain = new BattleRoleStateDomain();
            RoleStateRendererDomain = new BattleRoleStateRendererDomain();

            InputDomain = new BattleInputDomain();

            PhysicsDomain = new PhysicsDomain();

            ItemDomain = new ItemDomain();

            BulletLogicDomain = new BulletDomain();

            BulletRendererDomain = new BulletRendererDomain();
            BulletItemDomain = new BulletItemDomain();

            WeaponDomain = new WeaponDomain();
            WeaponItemDomain = new WeaponItemDomain();

            ArmorDomain = new BattleArmorDomain();
            ArmorItemDomain = new BattleArmorItemDomain();
            ArmorEvolveItemDomain = new BattleArmorEvolveItemDomain();

            HitDomain = new BattleHitDomain();

            GameStateDomain = new BattleGameStateDomain();

            FieldDomain = new BattleFieldDomain();

        }

        // todo: obsolete
        public void Inject(BattleFacades facades)
        {
            SceneDomain.Inject(facades);

            RoleDomain.Inject(facades);
            RoleRendererDomain.Inject(facades);
            RoleStateDomain.Inject(facades);
            RoleStateRendererDomain.Inject(facades);

            InputDomain.Inject(facades);

            BulletLogicDomain.Inject(facades);
            BulletRendererDomain.Inject(facades);
            BulletItemDomain.Inject(facades);

            ItemDomain.Inject(facades);

            PhysicsDomain.Inject(facades);

            WeaponDomain.Inject(facades);
            WeaponItemDomain.Inject(facades);

            ArmorDomain.Inject(facades);
            ArmorItemDomain.Inject(facades);
            ArmorEvolveItemDomain.Inject(facades);

            HitDomain.Inject(facades);

            GameStateDomain.Inject(facades);

            FieldDomain.Inject(facades);

        }
    }

}
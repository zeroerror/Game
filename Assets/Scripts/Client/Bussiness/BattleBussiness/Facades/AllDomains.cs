using Game.Client.Bussiness.BattleBussiness.Controller.Domain;

namespace Game.Client.Bussiness.BattleBussiness.Facades
{

    public class AllDomains
    {

        public BattleRoleLogicDomain RoleLogicDomain { get; private set; }
        public BattleRoleRendererDomain RoleRendererDomain { get; private set; }
        public BattleRoleStateDomain RoleStateDomain { get; private set; }
        public BattleRoleStateRendererDomain RoleStateRendererDomain { get; private set; }

        public BattleInputDomain InputDomain { get; private set; }

        public BattlePhysicsDomain PhysicsDomain { get; private set; }

        public BattleItemDomain ItemDomain { get; private set; }

        public BattleBulletLogicDomain BulletLogicDomain { get; private set; }
        public BattleBulletRendererDomain BulletRendererDomain { get; private set; }
        public BattleBulletItemDomain BulletItemDomain { get; private set; }

        public BattleWeaponDomain WeaponDomain { get; private set; }
        public BattleWeaponItemDomain WeaponItemDomain { get; private set; }

        public BattleArmorDomain ArmorDomain { get; private set; }
        public BattleArmorItemDomain ArmorItemDomain { get; private set; }

        public BattleEvolveItemDomain EvolveItemDomain { get; private set; }

        public BattleHitDomain HitDomain { get; private set; }

        public BattleStateDomain BattleStateDomain { get; private set; }

        public BattleFieldDomain FieldDomain { get; private set; }

        public BattleCommonDomain CommonDomain { get; private set; }

        public BattleAirdropLogicDomain AirdropLogicDomain { get; private set; }

        public BattleAirdropRendererDomain AirdropRendererDomain { get; private set; }

        public AllDomains()
        {
            RoleLogicDomain = new BattleRoleLogicDomain();
            RoleRendererDomain = new BattleRoleRendererDomain();
            RoleStateDomain = new BattleRoleStateDomain();
            RoleStateRendererDomain = new BattleRoleStateRendererDomain();

            InputDomain = new BattleInputDomain();

            PhysicsDomain = new BattlePhysicsDomain();

            ItemDomain = new BattleItemDomain();

            BulletLogicDomain = new BattleBulletLogicDomain();

            BulletRendererDomain = new BattleBulletRendererDomain();
            BulletItemDomain = new BattleBulletItemDomain();

            WeaponDomain = new BattleWeaponDomain();
            WeaponItemDomain = new BattleWeaponItemDomain();

            ArmorDomain = new BattleArmorDomain();
            ArmorItemDomain = new BattleArmorItemDomain();

            EvolveItemDomain = new BattleEvolveItemDomain();

            HitDomain = new BattleHitDomain();

            BattleStateDomain = new BattleStateDomain();

            FieldDomain = new BattleFieldDomain();

            CommonDomain = new BattleCommonDomain();

            AirdropLogicDomain = new BattleAirdropLogicDomain();

            AirdropRendererDomain = new BattleAirdropRendererDomain();
        }

        public void Inject(BattleFacades facades)
        {
            RoleLogicDomain.Inject(facades);
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
            EvolveItemDomain.Inject(facades);

            HitDomain.Inject(facades);

            AirdropRendererDomain.Inject(facades);

            BattleStateDomain.Inject(facades);

            FieldDomain.Inject(facades);

            CommonDomain.Inject(facades);

            AirdropLogicDomain.Inject(facades);
        }
    }

}
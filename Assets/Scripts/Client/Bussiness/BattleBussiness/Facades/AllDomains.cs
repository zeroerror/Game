using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;

namespace Game.Client.Bussiness.BattleBussiness.Facades
{

    public class AllDomains
    {

        public BattleSpawnDomain SpawnDomain { get; private set; }
        public BattleRoleDomain RoleDomain { get; private set; }
        public BattleRoleStateDomain RoleStateDomain { get; private set; }
        public BattleInputDomain InputDomain { get; private set; }
        public BulletDomain BulletDomain { get; private set; }
        public ItemDomain ItemDomain { get; private set; }
        public PhysicsDomain PhysicsDomain { get; private set; }
        public WeaponDomain WeaponDomain { get; private set; }
        public BattleHitDomain HitDomain { get; private set; }
        public BattleRendererDomain RendererDomain { get; private set; }


        public AllDomains()
        {
            SpawnDomain = new BattleSpawnDomain();
            RoleDomain = new BattleRoleDomain();
            RoleStateDomain = new BattleRoleStateDomain();
            InputDomain = new BattleInputDomain();
            BulletDomain = new BulletDomain();
            ItemDomain = new ItemDomain();
            PhysicsDomain = new PhysicsDomain();
            WeaponDomain = new WeaponDomain();
            HitDomain = new BattleHitDomain();
            RendererDomain = new BattleRendererDomain();
        }

        // todo: obsolete
        public void Inject(BattleFacades facades)
        {
            SpawnDomain.Inject(facades);
            RoleDomain.Inject(facades);
            RoleStateDomain.Inject(facades);
            InputDomain.Inject(facades);
            BulletDomain.Inject(facades);
            ItemDomain.Inject(facades);
            PhysicsDomain.Inject(facades);
            WeaponDomain.Inject(facades);
            HitDomain.Inject(facades);
            RendererDomain.Inject(facades);
        }
    }

}
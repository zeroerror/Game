using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;

namespace Game.Client.Bussiness.BattleBussiness.Facades
{

    public class AllDomains
    {

        public BattleSpawnDomain BattleSpawnDomain { get; private set; }
        public BattleRoleDomain BattleRoleDomain { get; private set; }
        public BattleInputDomain BattleInputDomain { get; private set; }
        public BulletDomain BulletDomain { get; private set; }
        public ItemDomain ItemDomain { get; private set; }
        public PhysicsDomain PhysicsDomain { get; private set; }
        public WeaponDomain WeaponDomain { get; private set; }
        

        public AllDomains()
        {
            BattleSpawnDomain = new BattleSpawnDomain();
            BattleRoleDomain = new BattleRoleDomain();
            BattleInputDomain = new BattleInputDomain();
            BulletDomain = new BulletDomain();
            ItemDomain = new ItemDomain();
            PhysicsDomain = new PhysicsDomain();
            WeaponDomain = new WeaponDomain();
        }

        // todo: obsolete
        public void Inject(BattleFacades facades)
        {
            BattleSpawnDomain.Inject(facades);
            BattleRoleDomain.Inject(facades);
            BulletDomain.Inject(facades);
            ItemDomain.Inject(facades);
            PhysicsDomain.Inject(facades);
            WeaponDomain.Inject(facades);
        }
    }

}
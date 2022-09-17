using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.WorldBussiness.Controller.Domain;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllDomains
    {

        public WorldSpawnDomain WorldSpawnDomain { get; private set; }
        public WorldRoleDomain WorldRoleDomain { get; private set; }
        public WorldInputDomain WorldInputDomain { get; private set; }
        public BulletDomain BulletDomain { get; private set; }
        public WeaponDomain WeaponDomain { get; private set; }
        public PhysicsDomain PhysicsDomain { get; private set; }

        public AllDomains()
        {
            WorldSpawnDomain = new WorldSpawnDomain();
            WorldRoleDomain = new WorldRoleDomain();
            WorldInputDomain = new WorldInputDomain();
            BulletDomain = new BulletDomain();
            WeaponDomain = new WeaponDomain();
            PhysicsDomain = new PhysicsDomain();
        }

        // todo: obsolete
        public void Inject(WorldFacades facades)
        {
            WorldSpawnDomain.Inject(facades);
            WorldRoleDomain.Inject(facades);
            BulletDomain.Inject(facades);
            WeaponDomain.Inject(facades);
            PhysicsDomain.Inject(facades);
        }
    }

}
using Game.Client.Bussiness.WorldBussiness.Facades ;
using Game.Client.Bussiness.WorldBussiness.Controller.Domain;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllDomains
    {

        public WorldSpawnDomain WorldSpawnDomain { get; private set; }
        public WorldRoleSpawnDomain WorldRoleSpawnDomain { get; private set; }
        public BulletDomain BulletDomain { get; private set; }
        public PhysicsDomain PhysicsDomain { get; private set; }

        public AllDomains()
        {
            WorldSpawnDomain = new WorldSpawnDomain();
            WorldRoleSpawnDomain = new WorldRoleSpawnDomain();
            BulletDomain = new BulletDomain();
            PhysicsDomain = new PhysicsDomain();
        }

        public void Inject(WorldFacades facades){
            WorldSpawnDomain.Inject(facades);
            WorldRoleSpawnDomain.Inject(facades);
            BulletDomain.Inject(facades);
            PhysicsDomain.Inject(facades);
        }
    }

}
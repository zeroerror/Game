using Game.Client.Bussiness.WorldBussiness.Facades ;
using Game.Client.Bussiness.WorldBussiness.Controller.Domain;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllDomains
    {

        public WorldSpawnDomain WorldSpawnDomain { get; private set; }
        public WorldRoleSpawnDomain WorldRoleSpawnDomain { get; private set; }
        public BulletSpawnDomain BulletSpawnDomain { get; private set; }

        public AllDomains()
        {
            WorldSpawnDomain = new WorldSpawnDomain();
            WorldRoleSpawnDomain = new WorldRoleSpawnDomain();
            BulletSpawnDomain = new BulletSpawnDomain();
        }

        public void Inject(WorldFacades facades){
            WorldSpawnDomain.Inject(facades);
            WorldRoleSpawnDomain.Inject(facades);
            BulletSpawnDomain.Inject(facades);
        }
    }

}
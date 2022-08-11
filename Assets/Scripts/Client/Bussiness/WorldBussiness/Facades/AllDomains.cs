using Game.Client.Bussiness.WorldBussiness.Facades ;
using Game.Client.Bussiness.WorldBussiness.Controller.Domain;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllDomains
    {

        public WorldRoleSpawnDomain WorldRoleSpawnDomain { get; private set; }
        public WorldSpawnDomain WorldSpawnDomain { get; private set; }

        public AllDomains()
        {
            WorldRoleSpawnDomain = new WorldRoleSpawnDomain();
            WorldSpawnDomain = new WorldSpawnDomain();
        }

        public void Inject(WorldFacades facades){
            WorldRoleSpawnDomain.Inject(facades);
            WorldSpawnDomain.Inject(facades);
        }
    }

}
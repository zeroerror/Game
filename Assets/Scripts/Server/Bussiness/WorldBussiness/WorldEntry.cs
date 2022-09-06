using Game.Infrastructure.Network.Server;
using Game.Server.Bussiness.WorldBussiness.Facades;

namespace Game.Server.Bussiness.WorldBussiness
{

    public class WorldEntry
    {
        WorldFacades worldFacades;

        WorldController worldController;

        public WorldEntry()
        {
            worldFacades = new WorldFacades();
            worldController = new WorldController();
        }

        public void Inject(NetworkServer server)
        {
            // Facades
            worldFacades.Inject(server);

            // Conntroller
            worldController.Inject(worldFacades);
        }

        public void Init()
        {
            worldFacades.ClientWorldFacades.Init();
        }

        public void Tick()
        {
            worldController.Tick();
        }


    }

}
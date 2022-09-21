using Game.Infrastructure.Network.Server;
using Game.Server.Bussiness.WorldBussiness.Facades;

namespace Game.Server.Bussiness.WorldBussiness
{

    public class WorldEntry
    {
        WorldFacades worldFacades;

        WorldController worldController;
        WorldPhysicsController worldPhysicsController;
        WorldNetworkController worldNetworkController;

        public WorldEntry()
        {
            worldFacades = new WorldFacades();
            worldController = new WorldController();
            worldPhysicsController = new WorldPhysicsController();
            worldNetworkController = new WorldNetworkController();
        }

        public void Inject(NetworkServer server, float fixedDeltaTime)
        {
            // Facades
            worldFacades.Inject(server);

            // Conntroller
            worldController.Inject(worldFacades, fixedDeltaTime);
            worldPhysicsController.Inject(worldFacades, fixedDeltaTime);
            worldNetworkController.Inject(worldFacades, fixedDeltaTime);
        }

        public void Init()
        {
            worldFacades.ClientWorldFacades.Init();
        }

        public void Tick()
        {
            worldController.Tick();
            worldPhysicsController.Tick();
            worldNetworkController.Tick();
        }

    }

}
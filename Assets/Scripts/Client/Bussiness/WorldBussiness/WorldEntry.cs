using Game.Client.Bussiness.BattleBussiness.Controller;
using Game.Client.Bussiness.WorldBussiness.Controller;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WorldEntry
    {

        // Facades
        WorldFacades worldFacades;

        // Controller
        WorldController worldController;

        public WorldEntry() { }

        public void Ctor()
        {
            // == Facades ==
            worldFacades = new WorldFacades();
            // == Controller ==
            worldController = new WorldController();
        }

        public void Inject(NetworkClient client)
        {
            // == Facades ==
            worldFacades.Inject(client);
            worldController.Inject(worldFacades);
            // == Controller ==

        }

        public void Tick()
        {
            // == Controller ==
            worldController.Tick();
        }

        public void TearDown()
        {
            // Send Disconnection
            worldFacades.Network.WorldReqAndRes.SendReq_LeaveWorld();
        }

    }

}
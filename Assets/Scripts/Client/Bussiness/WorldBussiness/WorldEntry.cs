using Game.Client.Bussiness.BattleBussiness.Controller;
using Game.Client.Bussiness.WorldBussiness.Controller;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness
{

    public static class WorldEntry
    {

        // Facades
        static WorldFacades worldFacades;

        // Controller
        static WorldController worldController;

        #region [Life Cycle]

        public static void Ctor()
        {
            // == Facades ==
            worldFacades = new WorldFacades();
            // == Controller ==
            worldController = new WorldController();
        }

        public static void Inject(NetworkClient client)
        {
            // == Facades ==
            worldFacades.Inject(client);
            worldController.Inject(worldFacades);
            // == Controller ==

        }

        public static void Tick()
        {
            // == Controller ==
            worldController.Tick();
        }

        public static void TearDown()
        {
            // Send Disconnection
            worldFacades.Network.WorldReqAndRes.SendReq_WorldLeaveMsg();
        }

        #endregion

    }

}
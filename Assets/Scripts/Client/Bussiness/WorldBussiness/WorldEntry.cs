using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.WorldBussiness.Controller;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness
{

    public static class WorldEntry
    {

        #region [Life Cycle]

        public static void Ctor()
        {
            // == Asset ==
            AllWorldAsset.Ctor();
        }

        public static void Init()
        {

        }

        public static void Inject(NetworkClient client)
        {
            // == Network ==

            // == Controller ==
            WorldController.Inject(client, AllWorldAsset.WorldReqAndRes);
            WorldRoleController.Inject(client, AllWorldAsset.WorldRoleReqAndRes);
        }

        public static void Tick()
        {
            // == Controller ==
            WorldController.Tick();
            WorldRoleController.Tick();
        }

        public static void TearDown()
        {

        }

        #endregion

    }

}
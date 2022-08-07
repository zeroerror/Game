
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.LoginBussiness.Facades;
using Game.Client.Bussiness.LoginBussiness.Controllers;

namespace Game.Client.Bussiness.LoginBussiness
{

    public static class LoginEntry
    {

        #region [Life Cycle]

        public static void Ctor()
        {
            // == Asset ==
            AllLoginAsset.Ctor();
        }

        public static void Init()
        {

        }

        public static void Inject(NetworkClient client)
        {
            // == Network ==

            // == Controller ==
            LoginController.Inject(client);
        }

        public static void Tick()
        {
            // == Controller ==
            LoginController.Tick();
        }

        public static void TearDown()
        {

        }

        #endregion

    }

}

using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.LoginBussiness.Facades;
using Game.Client.Bussiness.LoginBussiness.Controllers;

namespace Game.Client.Bussiness.LoginBussiness
{

    public static class LoginEntry
    {

        static LoginController loginController;

        #region [Life Cycle]

        public static void Ctor()
        {
            // == Asset ==
            AllLoginAsset.Ctor();

            // == Controller ==
            loginController = new LoginController();
        }

        public static void Init()
        {

        }

        public static void Inject(NetworkClient client)
        {
            // == Network ==

            // == Controller ==
            loginController.Inject(client);
        }

        public static void Tick()
        {
            // == Controller ==
            loginController.Tick();
        }

        public static void TearDown()
        {

        }

        #endregion

    }

}
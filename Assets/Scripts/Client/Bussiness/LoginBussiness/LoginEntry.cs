
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.LoginBussiness.Facades;
using Game.Client.Bussiness.LoginBussiness.Controllers;

namespace Game.Client.Bussiness.LoginBussiness
{

    public class LoginEntry
    {

        LoginController loginController;

        #region [Life Cycle]
        public LoginEntry()
        {

        }

        public void Ctor()
        {
            // == Asset ==
            AllLoginAsset.Ctor();

            // == Controller ==
            loginController = new LoginController();
        }

        public void Inject(NetworkClient client)
        {
            // == Network ==

            // == Controller ==
            loginController.Inject(client);
        }

        public void Tick()
        {
            // == Controller ==
            loginController.Tick();
        }

        public void TearDown()
        {

        }

        #endregion

    }

}
using Game.Infrastructure.Network.Server;
using Game.Server.Bussiness.LoginBussiness.Facades;

namespace Game.Server.Bussiness.LoginBussiness
{

    public class LoginEntry
    {

        LoginFacades loginFacades;
        LoginController loginController;

        public LoginEntry()
        {
            loginFacades = new LoginFacades();
            loginController = new LoginController();
        }

        public void Inject(NetworkServer server)
        {
            loginFacades.Inject(server);

            loginController.Inject(loginFacades);
        }

        public void Tick()
        {
            loginController.Tick();
        }

    }


}
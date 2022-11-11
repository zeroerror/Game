using Game.Infrastructure.Network.Server;
using Game.Server.Bussiness.LoginBussiness.Facades;

namespace Game.Server.Bussiness.LoginBussiness
{

    public class LoginEntry
    {

        ServerLoginFacades loginFacades;
        LoginController loginController;

        public LoginEntry()
        {
            loginController = new LoginController();
        }

        public void Inject(ServerLoginFacades facades)
        {
            loginFacades = facades;
            loginController.Inject(loginFacades);
        }

        public void Tick()
        {
            loginController.Tick();
        }

    }


}
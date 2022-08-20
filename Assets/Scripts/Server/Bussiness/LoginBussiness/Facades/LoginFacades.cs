
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.LoginBussiness.Facades
{

    public class LoginFacades
    {

        public NetworkServer NetworkServer { get; private set; }

        public LoginFacades()
        {
        }

        public void Inject(NetworkServer server)
        {
            this.NetworkServer = server;
        }

    }

}
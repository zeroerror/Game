
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.LoginBussiness.Facades
{

    public class ServerLoginFacades
    {

        public NetworkServer NetworkServer { get; private set; }

        public ServerLoginFacades()
        {
        }

        public void Inject(NetworkServer server)
        {
            this.NetworkServer = server;
        }

    }

}
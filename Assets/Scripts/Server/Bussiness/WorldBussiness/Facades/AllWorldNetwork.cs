
using Game.Server.Bussiness.WorldBussiness.Network;
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.WorldBussiness.Facades
{

    public class AllWorldNetwork
    {

        // Network
        public WorldRoleReqAndRes WorldRoleReqAndRes { get; private set; }

        public AllWorldNetwork()
        {
            WorldRoleReqAndRes = new WorldRoleReqAndRes();
        }

        public void Inject(NetworkServer server)
        {
            WorldRoleReqAndRes.Inject(server);
        }

    }

}
using Game.Client.Bussiness.WorldBussiness.Network;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllWorldNetwork
    {

        // Network
        public WorldReqAndRes WorldReqAndRes { get; private set; }
        public WorldRoleReqAndRes WorldRoleReqAndRes { get; private set; }

        public AllWorldNetwork()
        {
            WorldReqAndRes = new WorldReqAndRes();
            WorldRoleReqAndRes = new WorldRoleReqAndRes();
        }

        public void Inject(NetworkClient client)
        {
            WorldReqAndRes.Inject(client);
            WorldRoleReqAndRes.Inject(client);
        }

    }

}
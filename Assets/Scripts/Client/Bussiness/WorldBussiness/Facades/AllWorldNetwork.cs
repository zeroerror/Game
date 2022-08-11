using Game.Client.Bussiness.WorldBussiness.Network;

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

    }

}
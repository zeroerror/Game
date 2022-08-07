using Game.Client.Bussiness.WorldBussiness.Network;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public static class WorldRoleController
    {

        static WorldRoleReqAndRes _worldRoleReqAndRes;

        public static void Inject(NetworkClient client, WorldRoleReqAndRes worldRoleReqAndRes)
        {
            _worldRoleReqAndRes = worldRoleReqAndRes;
            _worldRoleReqAndRes.Inject(client);
        }

        public static void Tick()
        {

        }

    }

}
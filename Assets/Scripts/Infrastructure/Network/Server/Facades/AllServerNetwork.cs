using UnityEngine;

namespace Game.Infrastructure.Network.Server.Facades
{

    public static class AllServerNetwork
    {
        public static NetworkServer networkServer;

        public static void Ctor()
        {
            networkServer = new NetworkServer(4096);
        }


    }

}
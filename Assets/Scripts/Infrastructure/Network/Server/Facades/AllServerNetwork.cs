using UnityEngine;

namespace Game.Infrastructure.Network.Server.Facades
{
    // TODO:登录服、世界服、战斗服区分
    public class AllServerNetwork
    {
        public NetworkServer networkServer;

        public AllServerNetwork()
        {
            networkServer = new NetworkServer(4096);
        }


    }

}
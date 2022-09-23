using UnityEngine;

namespace Game.Infrastructure.Network.Server.Facades
{
    // TODO:登录服、世界服、战斗服区分
    public class AllServerNetwork
    {
        public NetworkServer LoginServer { get; private set; }
        public NetworkServer WorldServer { get; private set; }
        public NetworkServer BattleServer { get; private set; }

        public AllServerNetwork()
        {
            LoginServer = new NetworkServer(4096);
            WorldServer = new NetworkServer(4096);
            BattleServer = new NetworkServer(4096);
        }


    }

}
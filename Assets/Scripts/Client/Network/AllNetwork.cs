using Game.Network;

namespace Game.Client.Network
{

    public static class AllNetwork
    {
        public static NetworkClient networkClient;
        public static NetworkServer networkServer;

        public static void Ctor()
        {
            networkClient = new NetworkClient(1026);
            networkServer = new NetworkServer(4096);
        }


    }

}
using Game.Infrastructure.Network.Client;

namespace Game.Infrastructure.Network.Client.Facades
{

    public static class AllClientNetwork
    {
        public static NetworkClient networkClient;

        public static void Ctor()
        {
            networkClient = new NetworkClient(1026);
        }


    }

}
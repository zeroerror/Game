namespace Game.Infrastructure.Network.Client.Facades
{

    public static class AllClientNetwork
    {
        public static NetworkClient loginSerClient;
        public static NetworkClient worldSerClient;
        public static NetworkClient battleSerClient;

        public static void Ctor()
        {
            loginSerClient = new NetworkClient(1026);
            worldSerClient = new NetworkClient(1026);
            battleSerClient = new NetworkClient(1026);
        }


    }

}
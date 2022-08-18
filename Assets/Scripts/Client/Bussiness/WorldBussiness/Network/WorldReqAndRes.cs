using Game.Infrastructure.Network.Client;
using Game.Protocol.Client2World;
using Game.Protocol.World;

namespace Game.Client.Bussiness.WorldBussiness.Network
{

    public class WorldReqAndRes
    {
        NetworkClient _client;
        public void Inject(NetworkClient client)
        {
            _client = client;
        }

    }

}
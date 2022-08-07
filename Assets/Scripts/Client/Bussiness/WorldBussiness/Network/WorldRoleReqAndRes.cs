using Game.Infrastructure.Network.Client;
using Game.Protocol.Client2World;

namespace Game.Client.Bussiness.WorldBussiness.Network
{

    public class WorldRoleReqAndRes
    {
        NetworkClient _client;
        public void Inject(NetworkClient client)
        {
            _client = client;
        }

        public void SendWorldEnterReq()
        {

        }

        public void RegistWorldRoleBCRes()
        {

        }

    }

}
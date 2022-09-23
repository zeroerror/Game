

using Game.Infrastructure.Input;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.WorldBussiness.Network;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{


    public class AllWorldNetwork
    {

        public WorldReqAndRes WorldReqAndRes { get; private set; }

        public AllWorldNetwork()
        {
            WorldReqAndRes = new WorldReqAndRes();
        }

        public void Inject(NetworkClient client)
        {
            WorldReqAndRes.Inject(client);
        }

    }


}
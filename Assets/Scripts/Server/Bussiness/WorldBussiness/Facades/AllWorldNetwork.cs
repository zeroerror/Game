using Game.Infrastructure.Input;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;
using Game.Infrastructure.Network.Client;
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.WorldBussiness.Facades
{

    public class AllWorldNetwork
    {

        public WorldReqAndRes WorldReqAndRes { get; private set; }

        public AllWorldNetwork()
        {
            WorldReqAndRes = new WorldReqAndRes();
        }

        public void Inject(NetworkServer _server)
        {
            WorldReqAndRes.Inject(_server);
        }

        public void Init()
        {
        }

    }

}
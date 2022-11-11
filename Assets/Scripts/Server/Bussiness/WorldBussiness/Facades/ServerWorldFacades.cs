using Game.Infrastructure.Input;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;
using Game.Infrastructure.Network.Client;
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.WorldBussiness.Facades
{

    public class ServerWorldFacades
    {

        public AllWorldNetwork Network { get; private set; }
        public Game.Client.Bussiness.WorldBussiness.Facades.WorldFacades WorldFacades { get; private set; }

        public ServerWorldFacades()
        {
            Network = new AllWorldNetwork();
            WorldFacades = new Client.Bussiness.WorldBussiness.Facades.WorldFacades();
        }

        public void Inject(NetworkServer server)
        {
            Network.Inject(server);
        }

    }

}

using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.WorldBussiness.Facades
{

    public class WorldFacades
    {

        public AllWorldNetwork AllWorldNetwork { get; private set; }
        public Game.Client.Bussiness.WorldBussiness.Facades.WorldFacades ClientWorldFacades { get; private set; }

        public WorldFacades()
        {
            AllWorldNetwork = new AllWorldNetwork();
            ClientWorldFacades = new Client.Bussiness.WorldBussiness.Facades.WorldFacades();
        }

        public void Inject(NetworkServer server)
        {
            AllWorldNetwork.Inject(server);
        }

    }

}
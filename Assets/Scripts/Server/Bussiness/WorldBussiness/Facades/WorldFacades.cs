
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.WorldBussiness.Facades
{

    public class WorldFacades
    {

        public AllWorldNetwork AllWorldNetwork { get; private set; }

        public WorldFacades()
        {
            AllWorldNetwork = new AllWorldNetwork();
        }

        public void Inject(NetworkServer server)
        {
            AllWorldNetwork.Inject(server);
        }

    }

}
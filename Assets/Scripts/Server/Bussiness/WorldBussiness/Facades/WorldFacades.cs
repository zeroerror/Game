using Game.Infrastructure.Input;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;
using Game.Infrastructure.Network.Client;
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.WorldBussiness.Facades
{

    public class WorldFacades
    {

        public AllWorldNetwork Network { get; private set; }

        public WorldFacades()
        {
            Network = new AllWorldNetwork();
        }

        public void Inject(NetworkServer server)
        {
            Network.Inject(server);
        }

    }

}
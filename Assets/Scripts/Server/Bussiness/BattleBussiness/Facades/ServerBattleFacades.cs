
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.BattleBussiness.Facades
{

    public class ServerBattleFacades
    {

        public Game.Client.Bussiness.BattleBussiness.Facades.BattleFacades BattleFacades { get; private set; }

        public AllBattleNetwork Network { get; private set; }
        public LocalEventCenter LocalEventCenter { get; private set; }

        public ServerBattleFacades()
        {
            Network = new AllBattleNetwork();
            LocalEventCenter = new LocalEventCenter();
            BattleFacades = new Client.Bussiness.BattleBussiness.Facades.BattleFacades();
        }

        public void Inject(NetworkServer server)
        {
            Network.Inject(server);
        }

    }

}
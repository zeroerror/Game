
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.BattleBussiness.Facades
{

    public class ServerBattleFacades
    {

        public BattleFacades BattleFacades { get; private set; }

        public AllBattleNetwork Network { get; private set; }
        public LocalEventCenter LocalEventCenter { get; private set; }

        public ServerBattleFacades()
        {
            Network = new AllBattleNetwork();
            LocalEventCenter = new LocalEventCenter();
        }

        public void Inject(NetworkServer server, BattleFacades facades)
        {
            Network.Inject(server);
            BattleFacades = facades;
        }

    }

}
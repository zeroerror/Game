
using Game.Infrastructure.Network.Server;
using Game.Server.Bussiness.EventCenter;

namespace Game.Server.Bussiness.BattleBussiness.Facades
{

    public class BattleFacades
    {

        public AllBattleNetwork Network { get; private set; }
        public LocalEventCenter LocalEventCenter { get; private set; }

        public Game.Client.Bussiness.BattleBussiness.Facades.BattleFacades ClientBattleFacades { get; private set; }

        public BattleFacades()
        {
            Network = new AllBattleNetwork();
            LocalEventCenter = new LocalEventCenter();
            ClientBattleFacades = new Client.Bussiness.BattleBussiness.Facades.BattleFacades();
        }

        public void Inject(NetworkServer server)
        {
            Network.Inject(server);
        }

    }

}
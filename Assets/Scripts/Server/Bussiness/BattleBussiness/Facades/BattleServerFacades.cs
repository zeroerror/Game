
using Game.Infrastructure.Network.Server;
using Game.Server.Bussiness.EventCenter;

namespace Game.Server.Bussiness.BattleBussiness.Facades
{

    public class BattleServerFacades
    {

        public AllBattleNetwork Network { get; private set; }
        public LocalEventCenter LocalEventCenter { get; private set; }

        public Game.Client.Bussiness.BattleBussiness.Facades.BattleFacades BattleFacades { get; private set; }

        public BattleServerFacades()
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
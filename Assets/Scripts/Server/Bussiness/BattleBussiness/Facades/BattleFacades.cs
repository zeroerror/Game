
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.BattleBussiness.Facades
{

    public class BattleFacades
    {

        public AllBattleNetwork Network { get; private set; }
        public Game.Client.Bussiness.BattleBussiness.Facades.BattleFacades ClientBattleFacades { get; private set; }

        public BattleFacades()
        {
            Network = new AllBattleNetwork();
            ClientBattleFacades = new Client.Bussiness.BattleBussiness.Facades.BattleFacades();
        }

        public void Inject(NetworkServer server)
        {
            Network.Inject(server);
        }

    }

}
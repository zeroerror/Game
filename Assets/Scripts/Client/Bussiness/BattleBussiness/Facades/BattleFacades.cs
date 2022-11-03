using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.BattleBussiness.API;

namespace Game.Client.Bussiness.BattleBussiness.Facades
{

    public class BattleFacades
    {
        public AllBattleNetwork Network { get; private set; }

        public AllBattleRepo Repo { get; private set; }

        public AllDomains Domain { get; private set; }

        // Asset
        public AllBattleAssets Assets { get; private set; }

        // Controller Set
        public PlayerInputComponent InputComponent { get; private set; }

        // - Service
        public BattleArbitrationService ArbitrationService { get; private set; }
        public BattleLeagueService BattleLeagueService { get; private set; }
        public IDService IDService { get; private set; }

        // - API
        public LogicTriggerAPI LogicTriggerAPI { get; private set; }

        public BattleFacades()
        {
            Network = new AllBattleNetwork();

            Repo = new AllBattleRepo();

            Assets = new AllBattleAssets();

            Domain = new AllDomains();
            Domain.Inject(this);

            BattleLeagueService = new BattleLeagueService();
            IDService = new IDService();

            ArbitrationService = new BattleArbitrationService();
            ArbitrationService.Inject(this);

            LogicTriggerAPI = new LogicTriggerAPI();

            Assets.LoadAll();
        }

        public void Inject(NetworkClient client, PlayerInputComponent inputComponent)
        {
            Network.Inject(client);
            InputComponent = inputComponent;
        }

    }

}
using Game.Infrastructure.Input;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;
using Game.Infrastructure.Network.Client;

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

        public BattleArbitrationService ArbitrationService { get; private set; }

        public BattleLeagueService BattleLeagueService { get; private set; }

        public BattleFacades()
        {
            Network = new AllBattleNetwork();
            Repo = new AllBattleRepo();
            Assets = new AllBattleAssets();
            Domain = new AllDomains();
            
            ArbitrationService=new BattleArbitrationService();
            BattleLeagueService=new BattleLeagueService();

            ArbitrationService.Inject(this);
            Domain.Inject(this);

            // Asset Load
            Assets.LoadAll();
        }

        public void Inject(NetworkClient client, PlayerInputComponent inputComponent)
        {
            Network.Inject(client);
            InputComponent = inputComponent;
        }

    }

}
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
        public InputComponent InputComponent { get; private set; }

        public BattleFacades()
        {
            Network = new AllBattleNetwork();
            Repo = new AllBattleRepo();
            Domain = new AllDomains();
            Assets = new AllBattleAssets();
            Domain.Inject(this);
        }

        public void Inject(NetworkClient client, InputComponent inputComponent)
        {
            Network.Inject(client);
            InputComponent = inputComponent;
        }

        public void Init()
        {
            // Asset Load
            Assets.LoadAll();
        }

    }

}
using System.Collections.Generic;
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.BattleBussiness.API;
using Game.Client.Bussiness.BattleBussiness.Generic;

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

        // - Game Stage
        public BattleGameEntity GameEntity { get; private set; }

        // ====== 地图生成资源数据 ======
        public List<int> EntityIDList { get; private set; }
        public List<byte> ItemTypeByteList { get; private set; }
        public List<byte> SubTypeList { get; private set; }
        public List<EntityType> EntityTypeList { get; private set; }

        public BattleFacades()
        {
            Network = new AllBattleNetwork();

            Repo = new AllBattleRepo();

            Assets = new AllBattleAssets();
            Assets.LoadAll();

            Domain = new AllDomains();
            Domain.Inject(this);

            BattleLeagueService = new BattleLeagueService();
            IDService = new IDService();

            ArbitrationService = new BattleArbitrationService();
            ArbitrationService.Inject(this);

            LogicTriggerAPI = new LogicTriggerAPI();

            GameEntity = new BattleGameEntity();

            EntityIDList = new List<int>();
            ItemTypeByteList = new List<byte>();
            SubTypeList = new List<byte>();
            EntityTypeList = new List<EntityType>();
        }

        public void Inject(NetworkClient client, PlayerInputComponent inputComponent)
        {
            Network.Inject(client);
            InputComponent = inputComponent;
        }

    }

}
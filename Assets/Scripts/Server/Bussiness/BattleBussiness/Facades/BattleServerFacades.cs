
using System.Collections.Generic;
using Game.Infrastructure.Network.Server;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Server.Bussiness.BattleBussiness.Facades
{

    public class BattleServerFacades
    {

        public Game.Client.Bussiness.BattleBussiness.Facades.BattleFacades BattleFacades { get; private set; }

        public AllBattleNetwork Network { get; private set; }
        public LocalEventCenter LocalEventCenter { get; private set; }

        // ====== 地图生成资源数据 ======
        public List<int> EntityIDList { get; private set; }
        public List<byte> ItemTypeByteList { get; private set; }
        public List<byte> SubTypeList { get; private set; }
        public List<EntityType> EntityTypeList { get; private set; }

        public BattleServerFacades()
        {
            Network = new AllBattleNetwork();
            LocalEventCenter = new LocalEventCenter();
            BattleFacades = new Client.Bussiness.BattleBussiness.Facades.BattleFacades();
            EntityIDList = new List<int>();
            ItemTypeByteList = new List<byte>();
            SubTypeList = new List<byte>();
            EntityTypeList = new List<EntityType>();
        }

        public void Inject(NetworkServer server)
        {
            Network.Inject(server);
        }

    }

}
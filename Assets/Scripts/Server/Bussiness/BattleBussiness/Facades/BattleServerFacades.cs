
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
        public List<ushort> EntityIdList { get; private set; }
        public List<byte> ItemTypeByteList { get; private set; }
        public List<byte> SubTypeList { get; private set; }
        public List<ItemType> ItemTypeList { get; private set; }

        public BattleServerFacades()
        {
            Network = new AllBattleNetwork();
            LocalEventCenter = new LocalEventCenter();
            BattleFacades = new Client.Bussiness.BattleBussiness.Facades.BattleFacades();
            EntityIdList = new List<ushort>();
            ItemTypeByteList = new List<byte>();
            SubTypeList = new List<byte>();
            ItemTypeList = new List<ItemType>();
        }

        public void Inject(NetworkServer server)
        {
            Network.Inject(server);
        }

    }

}
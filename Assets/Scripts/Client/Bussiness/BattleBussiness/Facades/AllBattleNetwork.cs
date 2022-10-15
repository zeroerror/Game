using Game.Client.Bussiness.BattleBussiness.Network;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.BattleBussiness.Facades
{

    public class AllBattleNetwork
    {

        // Network
        public BattleRoleReqAndRes RoleReqAndRes { get; private set; }
        public BulletReqAndRes BulletReqAndRes { get; private set; }
        public WeaponReqAndRes WeaponReqAndRes { get; private set; }
        public ItemReqAndRes ItemReqAndRes { get; private set; }
        public BattleReqAndRes BattleReqAndRes { get; private set; }

        int clientFrame;

        public AllBattleNetwork()
        {
            RoleReqAndRes = new BattleRoleReqAndRes();
            BulletReqAndRes = new BulletReqAndRes();
            WeaponReqAndRes = new WeaponReqAndRes();
            ItemReqAndRes = new ItemReqAndRes();
            BattleReqAndRes = new BattleReqAndRes();
        }

        public void Inject(NetworkClient client)
        {
            RoleReqAndRes.Inject(client);
            BulletReqAndRes.Inject(client);
            WeaponReqAndRes.Inject(client);
            ItemReqAndRes.Inject(client);
            BattleReqAndRes.Inject(client);
        }

        public void Tick()
        {
            BattleReqAndRes.TickAllRegistAction();
            RoleReqAndRes.TickAllRegistAction();
            BulletReqAndRes.TickAllRegistAction();
            WeaponReqAndRes.TickAllRegistAction();
            ItemReqAndRes.TickAllRegistAction();
        }

    }

}
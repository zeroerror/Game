using Game.Client.Bussiness.WorldBussiness.Network;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllWorldNetwork
    {

        // Network
        public WorldRoleReqAndRes WorldRoleReqAndRes { get; private set; }
        public BulletReqAndRes BulletReqAndRes { get; private set; }
        public WeaponReqAndRes WeaponReqAndRes { get; private set; }
        public ItemReqAndRes ItemReqAndRes { get; private set; }

        int clientFrame;

        public AllWorldNetwork()
        {
            WorldRoleReqAndRes = new WorldRoleReqAndRes();
            BulletReqAndRes = new BulletReqAndRes();
            WeaponReqAndRes = new WeaponReqAndRes();
            ItemReqAndRes = new ItemReqAndRes();
        }

        public void Inject(NetworkClient client)
        {
            WorldRoleReqAndRes.Inject(client);
            BulletReqAndRes.Inject(client);
            WeaponReqAndRes.Inject(client);
            ItemReqAndRes.Inject(client);
        }

    }

}
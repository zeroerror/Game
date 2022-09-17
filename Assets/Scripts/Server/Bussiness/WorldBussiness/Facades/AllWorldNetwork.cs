
using Game.Server.Bussiness.WorldBussiness.Network;
using Game.Infrastructure.Network.Server;

namespace Game.Server.Bussiness.WorldBussiness.Facades
{

    public class AllWorldNetwork
    {

        // Network
        public WorldRoleReqAndRes WorldRoleReqAndRes { get; private set; }
        public BulletReqAndRes BulletReqAndRes { get; private set; }
        public WeaponReqAndRes WeaponReqAndRes { get; private set; }

        public AllWorldNetwork()
        {
            WorldRoleReqAndRes = new WorldRoleReqAndRes();
            BulletReqAndRes = new BulletReqAndRes();
            WeaponReqAndRes = new WeaponReqAndRes();
        }

        public void Inject(NetworkServer server)
        {
            WorldRoleReqAndRes.Inject(server);
            BulletReqAndRes.Inject(server);
            WeaponReqAndRes.Inject(server);
        }

    }

}
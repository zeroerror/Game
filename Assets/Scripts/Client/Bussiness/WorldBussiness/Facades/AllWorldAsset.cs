using Game.Client.Bussiness.WorldBussiness.Network;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public static class AllWorldAsset
    {

        // Network
        public static WorldReqAndRes WorldReqAndRes { get; private set; }
        public static WorldRoleReqAndRes WorldRoleReqAndRes { get; private set; }

        public static void Ctor()
        {
            WorldReqAndRes = new WorldReqAndRes();
            WorldRoleReqAndRes = new WorldRoleReqAndRes();
        }

    }

}
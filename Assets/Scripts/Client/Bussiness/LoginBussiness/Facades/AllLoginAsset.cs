using Game.Client.Bussiness.LoginBussiness.Network;

namespace Game.Client.Bussiness.LoginBussiness.Facades
{

    public static class AllLoginAsset
    {

        public static byte loginStatus;

        // Network
        public static LoginReqAndRes LoginReqAndRes;
        public static void Ctor()
        {
            LoginReqAndRes = new LoginReqAndRes();
        }

    }

}

namespace Game.Infrastructure.Generic
{

    public static class NetworkConfig
    {
        public static readonly string LOCAL_HOST = "localhost";
        public static readonly string LOGIN_HOST = "175.178.150.50";
        public static readonly ushort LOGIN_PORT = 4000;

        public static readonly string[] WORLDSERVER_HOST = new string[] { "175.178.150.50" };
        public static readonly ushort[] WORLDSERVER_PORT = new ushort[] { 4001 };

        public static readonly string[] BATTLESERVER_HOST = new string[] { "175.178.150.50" };
        public static readonly ushort[] BATTLESERVER_PORT = new ushort[] { 4002 };

        public static readonly float FIXED_DELTA_TIME = UnityEngine.Time.fixedDeltaTime;

    }

}
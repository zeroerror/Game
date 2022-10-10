
namespace Game.Infrastructure.Generic
{

    public static class NetworkConfig
    {
        // #if UNITY_EDITOR
        //         public static readonly string LOGIN_HOST = "localhost";
        // #else
        //         public static readonly string LOGIN_HOST = "175.178.150.50";
        // #endif
        //         public static readonly ushort LOGIN_PORT = 4000;

        // #if UNITY_EDITOR
        //         public static readonly string[] WORLDSERVER_HOST = new string[] { "localhost" };
        // #else
        //         public static readonly string[] WORLDSERVER_HOST = new string[] { "175.178.150.50" };
        // #endif
        //         public static readonly ushort[] WORLDSERVER_PORT = new ushort[] { 4001 };

        // #if UNITY_EDITOR
        //         public static readonly string[] BATTLESERVER_HOST = new string[] { "localhost" };
        // #else
        //         public static readonly string[] BATTLESERVER_HOST = new string[] { "175.178.150.50" };
        // #endif
        //         public static readonly ushort[] BATTLESERVER_PORT = new ushort[] { 4002 };

        //         public static readonly float FIXED_DELTA_TIME = UnityEngine.Time.fixedDeltaTime;

        public static readonly string LOGIN_HOST = "localhost";
        public static readonly ushort LOGIN_PORT = 4000;

        public static readonly string[] WORLDSERVER_HOST = new string[] { "localhost" };
        public static readonly ushort[] WORLDSERVER_PORT = new ushort[] { 4001 };

        public static readonly string[] BATTLESERVER_HOST = new string[] { "localhost" };
        public static readonly ushort[] BATTLESERVER_PORT = new ushort[] { 4002 };

        public static readonly float FIXED_DELTA_TIME = UnityEngine.Time.fixedDeltaTime;

    }

}
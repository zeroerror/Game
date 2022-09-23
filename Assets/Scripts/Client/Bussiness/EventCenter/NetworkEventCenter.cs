using System;
using Game.Protocol.Login;

namespace Game.Client.Bussiness.EventCenter
{

    public static class NetworkEventCenter
    {
        // == LoginBussiness ==
        static Action<string[], ushort[]> loginResHandler;
        public static void RegistLoginSuccess(Action<string[], ushort[]> action) => loginResHandler += action;
        public static void Invoke_LoginSuccessHandler(LoginResMessage msg) => loginResHandler.Invoke(msg.worldServerHosts, msg.worldServerPorts);

        static Action<string> worldConnResHandler;
        public static void RegistConnWorSerSuccess(Action<string> action) => worldConnResHandler += action;
        public static void Invoke_ConnWorSerSuccessHandler(string account) => worldConnResHandler.Invoke(account);

        public static void Ctor()
        {
        }

    }

}
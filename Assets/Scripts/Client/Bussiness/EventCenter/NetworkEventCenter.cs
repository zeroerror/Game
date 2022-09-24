using System;
using Game.Protocol.Login;

namespace Game.Client.Bussiness.EventCenter
{

    public static class NetworkEventCenter
    {
        // == LoginBussiness ==
        static Action<string, string[], ushort[]> loginResHandler;
        public static void RegistLoginSuccess(Action<string, string[], ushort[]> action) => loginResHandler += action;
        public static void Invoke_LoginSuccessHandler(LoginResMessage msg) => loginResHandler.Invoke(msg.account, msg.worldServerHosts, msg.worldServerPorts);

        static Action worldConnResHandler;
        public static void RegistConnWorSerSuccess(Action action) => worldConnResHandler += action;
        public static void Invoke_ConnWorSerSuccessHandler() => worldConnResHandler.Invoke();

        public static void Ctor()
        {
        }

    }

}
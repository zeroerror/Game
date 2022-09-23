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

        public static void Ctor()
        {
        }

        public static void Tick()
        {
        }

    }

}
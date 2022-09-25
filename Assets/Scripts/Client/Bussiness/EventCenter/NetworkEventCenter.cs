using System;
using Game.Protocol.Login;
using Game.Protocol.World;

namespace Game.Client.Bussiness.EventCenter
{

    public static class NetworkEventCenter
    {
        // == LoginBussiness ==
        static Action<string, string[], ushort[]> loginResHandler;
        public static void Regist_LoginSuccess(Action<string, string[], ushort[]> action) => loginResHandler += action;
        public static void Invoke_LoginSuccessHandler(LoginResMessage msg) => loginResHandler.Invoke(msg.account, msg.worldServerHosts, msg.worldServerPorts);

        // == WorldBussiness ==
        static Action worldConnResHandler;
        public static void Regist_ConnWorSerSuccess(Action action) => worldConnResHandler += action;
        public static void Invoke_ConnWorSerSuccessHandler() => worldConnResHandler.Invoke();

        static Action<string, string, string, ushort> worldRoomCreateResHandler;
        public static void Regist_WorldRoomCreate(Action<string, string, string, ushort> action) => worldRoomCreateResHandler += action;
        public static void Invoke_WorldRoomCreate(string masterAccount, string roomName, string host, ushort port) => worldRoomCreateResHandler.Invoke(masterAccount, roomName, host, port);

        static Action battleSerConnectHandler;
        public static void Regist_BattleSerConnectHandler(Action action) => battleSerConnectHandler += action;
        public static void Invoke_BattleSerConnectHandler() => battleSerConnectHandler.Invoke();


        public static void Ctor()
        {
        }

    }

}
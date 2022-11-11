using System;
using Game.Protocol.Login;
using Game.Protocol.World;

namespace Game.Client.Bussiness.EventCenter
{

    public static class NetworkEventCenter
    {
        // == LoginBussiness ==
        static Action<string, string[], ushort[]> loginResAction;
        public static void Regist_LoginSuccess(Action<string, string[], ushort[]> action) => loginResAction += action;
        public static void Invoke_LoginSuccessAction(LoginResMessage msg) => loginResAction.Invoke(msg.account, msg.worldServerHosts, msg.worldServerPorts);

        // == WorldBussiness ==
        static Action worldConnResAction;
        public static void Regist_ConnWorSerSuccess(Action action) => worldConnResAction += action;
        public static void Invoke_ConnWorSerSuccessAction() => worldConnResAction.Invoke();

        static Action<int[], string[], int[], int[], string[], string[], ushort[]> allWorldRoomsBasicInfoAction;
        public static void Regist_AllWorldRoomsBasicInfo(Action<int[], string[], int[], int[], string[], string[], ushort[]> action)
         => allWorldRoomsBasicInfoAction += action;
        public static void Invoke_AllWorldRoomsBasicInfo(WorldAllRoomsBacisInfoResMsg msg, string[] accountArray)
        => allWorldRoomsBasicInfoAction.Invoke(msg.worldRoomIDArray, msg.worldRoomNameArray, msg.worldRoomMemNums, msg.masterIDArray, accountArray, msg.hosts, msg.ports);

        static Action<string, string, string, ushort> worldRoomCreateResAction;
        public static void Regist_WorldRoomCreate(Action<string, string, string, ushort> action) => worldRoomCreateResAction += action;
        public static void Invoke_WorldRoomCreate(WorldCreateRoomResMsg msg) => worldRoomCreateResAction.Invoke(msg.roomName, msg.masterAccount, msg.host, msg.port);

        static Action<int> worldRoomDismissResAction;
        public static void Regist_WorldRoomDismiss(Action<int> action) => worldRoomDismissResAction += action;
        public static void Invoke_WorldRoomDismiss(WorldRoomDismissResMsg msg) => worldRoomDismissResAction.Invoke(msg.roomEntityID);

        static Action battleSerConnectAction;
        public static void Regist_BattleSerConnectAction(Action action) => battleSerConnectAction += action;
        public static void Invoke_BattleSerConnectAction() => battleSerConnectAction.Invoke();

        public static void Ctor()
        {
        }

    }

}
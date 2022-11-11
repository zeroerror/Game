using System;
using Game.Infrastructure.Network.Server;
using Game.Protocol.Login;

namespace Game.Server.Bussiness.EventCenter
{

    public static class ServerNetworkEventCenter
    {
        public static void Ctor()
        {
        }

        static Action<int> worldConnHandler;
        public static void Regist_WorldConnection(Action<int> action) => worldConnHandler += action;
        public static void Invoke_WorldConnection(int connId) => worldConnHandler.Invoke(connId);

        static Action<int> worldDisconnHandler;
        public static void Regist_WorldDisconnection(Action<int> action) => worldDisconnHandler += action;
        public static void Invoke_WorldDisconnection(int connId) => worldDisconnHandler.Invoke(connId);

        // == BattleServer
        static Action<string, ushort> startBattleServerAction;
        public static void Regist_StartBattleServer(Action<string, ushort> action) => startBattleServerAction += action;
        public static void Invoke_StartBattleServer(string host, ushort port) => startBattleServerAction.Invoke(host, port);

        static Action<int, NetworkServer> battleServerConnHandler;
        public static void Regist_BattleServerConnHandler(Action<int, NetworkServer> action) => battleServerConnHandler += action;
        public static void Invoke_BattleServerConnect(int connID, NetworkServer networkServer) => battleServerConnHandler.Invoke(connID, networkServer);


    }

}
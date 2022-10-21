using System;
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
        static Action battleServerCreateHandler;
        public static void Regist_BattleServerNeedCreate(Action action) => battleServerCreateHandler += action;
        public static void Invoke_BattleServerNeedCreate() => battleServerCreateHandler.Invoke();

        public static Action<int> battleSerConnect;

    }

}
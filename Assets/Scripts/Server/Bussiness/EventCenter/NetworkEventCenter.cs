using System;
using Game.Protocol.Login;

namespace Game.Server.Bussiness.EventCenter
{

    public static class NetworkEventCenter
    {
        public static void Ctor()
        {
        }

        public static Action<int> worldConnHandler;
        public static void Regist_WorldConnection(Action<int> action) => worldConnHandler += action;
        public static void Invoke_WorldConnection(int connId) => worldConnHandler.Invoke(connId);

        public static Action<int> worldDisconnHandler;
        public static void Regist_WorldDisconnection(Action<int> action) => worldDisconnHandler += action;
        public static void Invoke_WorldDisconnection(int connId) => worldDisconnHandler.Invoke(connId);



    }

}
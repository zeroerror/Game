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
        public static void Regist_NewWorldConnection(Action<int>  action) => worldConnHandler += action;
        public static void Invoke_NewWorldConnection(int connId) => worldConnHandler.Invoke(connId);



    }

}
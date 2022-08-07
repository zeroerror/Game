using System;
using UnityEngine;
using Game.Client.Bussiness.EventCenter.Facades;
using Game.Client.Bussiness.LoginBussiness.Facades;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Client2World;

namespace Game.Client.Bussiness.LoginBussiness.Controllers
{

    public static class LoginController
    {

        static event Action<byte> _loginHandler;
        static byte status;

        public static void Inject(NetworkClient client)
        {
            AllLoginAsset.LoginReqAndRes.Inject(client);
            AllLoginAsset.LoginReqAndRes.RegistLoginRes(OnLoginRes);
        }

        public static void Tick()
        {
            if (status != 0)
            {
                _loginHandler?.Invoke(status);
                status = 0;
            }
        }

        public static void SendLoginMsg(string account, string pwd)
        {
            AllLoginAsset.LoginReqAndRes.SendLoginMsg(account, pwd);
        }

        public static void AddLoginResRegist(Action<byte> action)
        {
            _loginHandler += action;
        }

        static void OnLoginRes(LoginResMessage msg)
        {
            if (msg.status == 1)
            {
                Debug.Log("登录成功!");
                AllBussinessEvent.LoginToWorldEvent.SetIsTrigger(true);
                status = msg.status;
            }
        }


    }

}
using System;
using UnityEngine;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.LoginBussiness.Facades;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Login;

namespace Game.Client.Bussiness.LoginBussiness.Controllers
{

    public static class LoginController
    {

        static event Action<byte> _loginHandler;
        static event Action<byte> _registAccountHandler;
        static byte loginTrigger;
        static byte registTrigger;

        public static void Inject(NetworkClient client)
        {
            AllLoginAsset.LoginReqAndRes.Inject(client);

            // 服务端事件监听
            AllLoginAsset.LoginReqAndRes.RegistLoginRes(OnLoginRes);
            AllLoginAsset.LoginReqAndRes.RegistRegistAccountRes(OnRegistAccountRes);
        }

        public static void Tick()
        {
            if (loginTrigger != 0)
            {
                _loginHandler?.Invoke(loginTrigger);
                loginTrigger = 0;
            }
            if (registTrigger != 0)
            {
                _registAccountHandler?.Invoke(registTrigger);
                registTrigger = 0;
            }
        }

        public static void SendLoginMsg(string account, string pwd)
        {
            AllLoginAsset.LoginReqAndRes.SendLoginMsg(account, pwd);
        }

        public static void AddRegister_LoginRes(Action<byte> action)
        {
            _loginHandler += action;
        }

        public static void SendRegistAccountMsg(string account, string pwd)
        {
            AllLoginAsset.LoginReqAndRes.SendRegistAccountMsg(account, pwd);
        }

        public static void AddRegister_RegistAccountRes(Action<byte> action)
        {
            _registAccountHandler += action;
        }

        // PRIVATE FUNC
        static void OnLoginRes(LoginResMessage msg)
        {
            if (msg.status == 1)
            {
                Debug.Log($"登录成功! 你的userToken:{msg.userToken}");
                NetworkEventCenter.InvokeLoginSuccessHandler();
                loginTrigger = msg.status;
            }else{

                Debug.Log("登录失败!账户或密码不正确！");
            }
        }

        static void OnRegistAccountRes(RegisterAccountResMessage msg)
        {
            if (msg.status == 1)
            {
                Debug.Log("注册成功!");
                registTrigger = msg.status;
            }else{
                Debug.Log("账户名已被注册!");
            }
        }


    }

}
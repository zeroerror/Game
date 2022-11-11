using System;
using UnityEngine;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.LoginBussiness.Facades;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Login;
using System.Collections.Generic;

namespace Game.Client.Bussiness.LoginBussiness.Controllers
{

    public class LoginController
    {

        public event Action<string[], ushort[]> _loginSuccessHandler;
        public event Action<byte> _registAccountHandler;

        Queue<LoginResMessage> loginResQueue;

        public LoginController()
        {
            loginResQueue = new Queue<LoginResMessage>();
            UIEventCenter.LoginAction += SendLoginMsg;
        }

        public void Inject(NetworkClient client)
        {
            AllLoginAsset.LoginReqAndRes.Inject(client);

            // 服务端事件监听
            AllLoginAsset.LoginReqAndRes.RegistLoginRes(OnLoginRes);
            AllLoginAsset.LoginReqAndRes.RegistRegistAccountRes(OnRegistAccountRes);
        }

        public void Tick()
        {

            while (loginResQueue.TryDequeue(out var msg))
            {
                NetworkEventCenter.Invoke_LoginSuccessAction(msg);
            }

        }

        public void SendLoginMsg(string account, string pwd)
        {
            AllLoginAsset.LoginReqAndRes.SendLoginMsg(account, pwd);
        }

        public void SendRegistAccountMsg(string account, string pwd)
        {
            AllLoginAsset.LoginReqAndRes.SendRegistAccountMsg(account, pwd);
        }

        // PRIVATE FUNC
        void OnLoginRes(LoginResMessage msg)
        {
            if (msg.status == 1)
            {
                Debug.Log($"登录成功! 你的userToken:{msg.userToken}");
                loginResQueue.Enqueue(msg);
            }
            else
            {
                Debug.Log("登录失败!账户或密码不正确！");
            }
        }

        void OnRegistAccountRes(RegisterAccountResMessage msg)
        {
            if (msg.status == 1)
            {
                Debug.Log("注册成功!");
            }
            else
            {
                Debug.Log("账户名已被注册!");
            }
        }


    }

}
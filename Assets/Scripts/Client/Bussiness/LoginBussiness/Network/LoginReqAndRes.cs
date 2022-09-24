using System;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Login;
using UnityEngine;
namespace Game.Client.Bussiness.LoginBussiness.Network
{

    public class LoginReqAndRes
    {
        NetworkClient _client;
        public void Inject(NetworkClient client)
        {
            _client = client;
        }

        public void SendLoginMsg(string name, string pwd)
        {
            LoginReqMessage msg = new LoginReqMessage
            {
                account = name,
                pwd = pwd
            };
            Debug.Log("Send Login Msg");
            _client.SendMsg<LoginReqMessage>(msg);
        }

        public void RegistLoginRes(Action<LoginResMessage> action)
        {
            _client.RegistMsg<LoginResMessage>(action);
        }

        public void SendRegistAccountMsg(string name, string pwd)
        {
            RegisterAccountReqMessage msg = new RegisterAccountReqMessage
            {
                name = name,
                pwd = pwd
            };
            Debug.Log("Send RegistAccount Msg");
            _client.SendMsg<RegisterAccountReqMessage>(msg);
        }

        public void RegistRegistAccountRes(Action<RegisterAccountResMessage> action)
        {
            _client.RegistMsg<RegisterAccountResMessage>(action);
        }

    }

}
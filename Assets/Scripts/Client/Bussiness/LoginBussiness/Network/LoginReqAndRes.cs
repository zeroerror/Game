using System;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Client2World;
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

        public void SendLoginMsg(string account, string pwd)
        {
            LoginReqMessage msg = new LoginReqMessage
            {
                account = account,
                pwd = pwd
            };
            Debug.Log("SendLoginMsg");
            _client.SendMsg<LoginReqMessage>(msg);
        }

        public void RegistLoginRes(Action<LoginResMessage> action)
        {
            _client.RegistMsg<LoginResMessage>(action);
        }

    }

}
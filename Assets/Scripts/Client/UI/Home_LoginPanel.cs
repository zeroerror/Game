using UnityEngine;
using UnityEngine.UI;
using Game.Protocol.Client2World;
using Game.Infrastructure;
using Game.Infrastructure.Network.Client.Facades;
using ZeroUIFrame;

namespace Game.UI
{

    public class Home_LoginPanel : UIBehavior
    {
        InputField _Account;
        InputField _Pwd;

        void Awake()
        {
            _Account = GetComponentFromChild<InputField>("AccountInputField");
            _Pwd = GetComponentFromChild<InputField>("PasswardInputField");

            SetOnClick("ConfirmBtn", SendLoginMsg);
        }

        void SendLoginMsg(params object[] args)
        {
            string account = _Account.text;
            string pwd = _Pwd.text;

            LoginReqMessage msg = new LoginReqMessage
            {
                account = account,
                pwd = pwd
            };

            var client = AllClientNetwork.networkClient;
            client.SendMessage(1, 1, msg);
            client.RegistMsg<LoginResMessage>(1, 1, (msg) =>
            {
                if (msg.status == 1)
                {
                    Debug.Log($"客户端：userToken={msg.userToken} 登陆成功-----------------------------");
                }
                else
                {
                    Debug.Log($"客户端：登陆失败-----------------------------");
                }
            });

        }



    }

}
using UnityEngine;
using UnityEngine.UI;
using ZeroUIFrame;
using Game.Client.Bussiness.LoginBussiness.Controllers;
using Game.UI.Event;

namespace Game.UI.Panel
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
            LoginController.AddLoginResRegist((status) =>
            {
                if (status == 1)
                {
                    UIEventCenter.EnqueueOpenQueue("Home_LoginPanel");
                }
            });
        }

        // == UI Click ==
        void SendLoginMsg(params object[] args)
        {
            LoginController.SendLoginMsg(_Account.text, _Pwd.text);
        }

    }

}
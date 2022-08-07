using UnityEngine;
using UnityEngine.UI;
using ZeroUIFrame;
using Game.Client.Bussiness.LoginBussiness.Controllers;

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
            LoginController.AddLoginResRegist((status) =>
            {
                if (status == 1)
                {
                    UIManager.CloseUI("Home_LoginPanel");
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
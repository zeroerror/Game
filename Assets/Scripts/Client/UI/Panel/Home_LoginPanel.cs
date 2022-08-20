using UnityEngine;
using UnityEngine.UI;
using ZeroUIFrame;
using Game.Client.Bussiness.LoginBussiness.Controllers;
using Game.UI.Event;

namespace Game.UI.Panel
{

    public class Home_LoginPanel : UIBehavior
    {
        InputField _name;
        InputField _Pwd;

        void Awake()
        {
            _name = GetComponentFromChild<InputField>("AccountInputField");
            _Pwd = GetComponentFromChild<InputField>("PasswardInputField");

            SetOnClick("LoginBtn", SendLoginMsg);
            SetOnClick("RegistBtn", SendRegistAccountMsg);
            LoginController.AddRegister_LoginRes((status) =>
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
            LoginController.SendLoginMsg(_name.text, _Pwd.text);
        }

        void SendRegistAccountMsg(params object[] args)
        {
            LoginController.SendRegistAccountMsg(_name.text, _Pwd.text);
        }

    }

}
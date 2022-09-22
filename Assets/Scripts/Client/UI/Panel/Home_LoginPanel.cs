using UnityEngine;
using UnityEngine.UI;
using ZeroUIFrame;
using Game.UI.Event;
using Game.Client.Bussiness.LoginBussiness.Controllers;
using Game.Client.Bussiness.EventCenter;

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

            NetworkEventCenter.RegistLoginSuccess(OnLoginSuccess);
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

        void OnLoginSuccess(string[] worldSerHosts, ushort[] ports)
        {
            Debug.Log($"UIController: 世界服 {worldSerHosts[0]}:{ports[0]}");
            UIEventCenter.EnqueueOpenQueue("Home_WorldServerPanel");
            UIEventCenter.EnqueueTearDownQueue("Home_LoginPanel");
        }

    }

}
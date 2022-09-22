using UnityEngine;
using UnityEngine.UI;
using ZeroUIFrame;
using Game.Bussiness.UIBussiness;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.UIBussiness.Panel
{

    public class Home_LoginPanel : UIBehavior
    {
        InputField _name;
        InputField _Pwd;
        UIEventCenter uIEventCenter;

        void Awake()
        {
            _name = GetComponentFromChild<InputField>("AccountInputField");
            _Pwd = GetComponentFromChild<InputField>("PasswardInputField");

            SetOnClick("LoginBtn", SendLoginMsg);
            SetOnClick("RegistBtn", SendRegistAccountMsg);

            uIEventCenter = args[0] as UIEventCenter;
            Debug.Assert(uIEventCenter != null);

            NetworkEventCenter.RegistLoginSuccess(OnLoginSuccess);
        }

        // == UI Click ==
        void SendLoginMsg(params object[] args)
        {
            LoginController.SendLoginMsg(_name.text, _Pwd.text);
        }

        void SendRegistAccountMsg(params object[] args)
        {
            // LoginController.SendRegistAccountMsg(_name.text, _Pwd.text);
        }

        void OnLoginSuccess(string[] worldSerHosts, ushort[] ports)
        {
            uIEventCenter.EnqueueTearDownQueue("Home_LoginPanel");
        }

    }

}
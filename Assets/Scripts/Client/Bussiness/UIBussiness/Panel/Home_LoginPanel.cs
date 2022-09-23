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

        void Awake()
        {
            _name = GetComponentFromChild<InputField>("AccountInputField");
            _Pwd = GetComponentFromChild<InputField>("PasswardInputField");
            SetOnClick("LoginBtn", ClickLoginBtn);
            SetOnClick("RegistBtn", ClickRegistBtn);

            NetworkEventCenter.RegistLoginSuccess(OnLoginSuccess);
        }

        // == UI Click ==
        void ClickLoginBtn(params object[] args)
        {
            UIEventCenter.LoginAction.Invoke(_name.text, _Pwd.text);
        }

        void ClickRegistBtn(params object[] args)
        {
            UIEventCenter.RegistAction.Invoke(_name.text, _Pwd.text);
        }

        void OnLoginSuccess(string[] worldSerHosts, ushort[] ports)
        {
            UIEventCenter.EnqueueTearDownQueue("Home_LoginPanel");
        }

    }

}
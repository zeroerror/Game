using UnityEngine;
using UnityEngine.UI;
using ZeroUIFrame;
using Game.Client.Bussiness.UIBussiness;
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
            OnPointerDown("LoginBtn", ClickLoginBtn);
            OnPointerDown("RegistBtn", ClickRegistBtn);
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

    }

}
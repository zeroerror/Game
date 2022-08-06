using UnityEngine;
using UnityEngine.UI;
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

            SetOnClick("ConfirmBtn", (obj) =>
            {
                Debug.Log($"_Account: {_Account.text}  _Pwd: {_Pwd.text}");

            });

        }



    }

}
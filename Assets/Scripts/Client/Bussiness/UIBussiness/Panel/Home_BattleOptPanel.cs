using UnityEngine;
using UnityEngine.UI;
using ZeroUIFrame;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.UIBussiness.Panel
{

    public class Home_BattleOptPanel : UIBehavior
    {

        VariableJoystick VariableJoystick;
        Button pick_b;
        Button fire_b;
        Button reload_b;
        Button jump_b;
        Button dropWeapon_b;

        void Awake()
        {
            VariableJoystick = transform.Find("Joystick").GetComponent<VariableJoystick>();
            fire_b = transform.Find("OptGroup/Fire").GetComponent<Button>();
            reload_b = transform.Find("OptGroup/Reload").GetComponent<Button>();
            jump_b = transform.Find("OptGroup/Jump").GetComponent<Button>();
            dropWeapon_b = transform.Find("OptGroup/Jump").GetComponent<Button>();

            VariableJoystick.moveHandler += OnMoveAxis;
            SetOnClick("OptGroup/Pick", OnClickPick_Button);
            SetOnClick("OptGroup/Fire", OnClickFire_Button);
            SetOnClick("OptGroup/Reload", OnClickReload_Button);
            SetOnClick("OptGroup/Jump", OnClickJump_Button);
            SetOnClick("OptGroup/DropWeapon", OnClickDropWeapon_Button);
        }

        void OnMoveAxis(Vector2 moveAxis)
        {
            UIEventCenter.MoveAction.Invoke(moveAxis);
        }

        void OnClickPick_Button(params object[] args)
        {
            UIEventCenter.PickAction.Invoke();
        }
        void OnClickFire_Button(params object[] args)
        {
            UIEventCenter.FireAction.Invoke();
        }
        void OnClickReload_Button(params object[] args)
        {
            UIEventCenter.ReloadAction.Invoke();
        }
        void OnClickJump_Button(params object[] args)
        {
            UIEventCenter.JumpAction.Invoke();
        }
        void OnClickDropWeapon_Button(params object[] args)
        {
            UIEventCenter.DropWeaponAction.Invoke();
        }

    }

}
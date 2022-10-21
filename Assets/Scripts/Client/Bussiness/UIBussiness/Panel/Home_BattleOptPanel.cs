using UnityEngine;
using UnityEngine.UI;
using ZeroUIFrame;
using Game.Client.Bussiness.EventCenter;
using UnityEngine.EventSystems;

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
            OnPointerDown("OptGroup/Pick", OnClickPick_Button);

            OnPointerDown("OptGroup/Fire", PointerDownFireBtn);
            OnPointerDrag("OptGroup/Fire", PointerDragFireBtn);
            OnPointerUp("OptGroup/Fire", PointerUpFireBtn);

            OnPointerDown("OptGroup/Reload", OnClickReload_Button);
            OnPointerDown("OptGroup/Jump", OnClickJump_Button);
            OnPointerDown("OptGroup/DropWeapon", OnClickDropWeapon_Button);
        }

        void OnMoveAxis(Vector2 moveAxis)
        {
            UIEventCenter.MoveAction.Invoke(moveAxis);
        }

        void OnClickPick_Button(params object[] args)
        {
            UIEventCenter.PickAction.Invoke();
        }

        void PointerDownFireBtn(params object[] args)
        {
            var evData = args[1] as PointerEventData;
            var fireDir = evData.delta;
            UIEventCenter.FireAction.Invoke(fireDir);
        }

        void PointerDragFireBtn(params object[] args)
        {
            var evData = args[1] as PointerEventData;
            var fireDir = evData.delta;
            UIEventCenter.FireAction.Invoke(fireDir);
        }

        void PointerUpFireBtn(params object[] args)
        {
            UIEventCenter.StopFireAction.Invoke();
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
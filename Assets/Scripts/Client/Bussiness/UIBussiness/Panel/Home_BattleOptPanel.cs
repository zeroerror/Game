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

        void Awake()
        {
            VariableJoystick = transform.Find("Joystick").GetComponent<VariableJoystick>();
            VariableJoystick.moveHandler += OnMoveAxis;
            
            OnPointerDown("OptGroup/Pick", OnClickPick_Button);

            OnPointerDown("OptGroup/Shoot", PointerDownShootBtn);
            OnPointerDrag("OptGroup/Shoot", PointerDragShootBtn);
            OnPointerUp("OptGroup/Shoot", PointerUpShootBtn);

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

        void PointerDownShootBtn(params object[] args)
        {
            var evData = args[1] as PointerEventData;
            var fireDir = (evData.position - evData.pressPosition).normalized;
            UIEventCenter.ShootAction.Invoke(fireDir);
        }

        void PointerDragShootBtn(params object[] args)
        {
            var evData = args[1] as PointerEventData;
            var fireDir = (evData.position - evData.pressPosition).normalized;
            UIEventCenter.ShootAction.Invoke(fireDir);
        }

        void PointerUpShootBtn(params object[] args)
        {
            UIEventCenter.StopShootAction.Invoke();
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
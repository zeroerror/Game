using UnityEngine;
using UnityEngine.UI;
using ZeroUIFrame;
using Game.Bussiness.UIBussiness;
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
            Debug.Assert(VariableJoystick != null);
            VariableJoystick.moveHandler += OnMoveAxis;
        }

        void OnMoveAxis(Vector2 moveAxis)
        {
            UIEventCenter.MoveAction.Invoke(moveAxis);
        }
        // == UI Click ==

    }

}
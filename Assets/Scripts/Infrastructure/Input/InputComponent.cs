
using UnityEngine;

public class InputComponent
{

    public Vector3 moveAxis;
    public Vector3 pressMouse0_Point;
    public bool pressJump;

    public InputComponent()
    {
        moveAxis = new Vector2();
    }

    public void Reset()
    {
        moveAxis.x = 0;
        moveAxis.y = 0;
        moveAxis.z = 0;

        pressMouse0_Point.x = 0;
        pressMouse0_Point.y = 0;
        pressMouse0_Point.z = 0;
        
        pressJump = false;
    }

}
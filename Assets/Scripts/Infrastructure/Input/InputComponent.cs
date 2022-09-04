
using UnityEngine;

public class InputComponent
{

    public Vector3 moveAxis;
    public Vector3 shootPoint;
    public Vector3 grenadeThrowPoint;
    public Vector3 hookPoint;
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

        shootPoint = Vector3.zero;
        grenadeThrowPoint = Vector3.zero;
        hookPoint = Vector3.zero;

        pressJump = false;
    }

}
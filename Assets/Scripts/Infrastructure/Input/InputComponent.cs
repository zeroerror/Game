
using UnityEngine;
using Game.Generic;

public class InputComponent
{

    public Vector3 moveAxis;
    public Vector2 mouseAxis;
    public Vector3 shootPoint;
    public Vector3 grenadeThrowPoint;
    public Vector3 hookPoint;
    public bool pressJump;
    public bool pressV;

    public InputComponent()
    {
        moveAxis = new Vector3();
        mouseAxis = new Vector2();
        shootPoint = new Vector3();
        grenadeThrowPoint = new Vector3();
        grenadeThrowPoint = new Vector3();
    }

    public void Reset()
    {
        moveAxis.Reset();
        shootPoint.Reset();
        grenadeThrowPoint.Reset();
        hookPoint.Reset();
        mouseAxis.Reset();

        pressV = false;
        pressJump = false;
    }

}
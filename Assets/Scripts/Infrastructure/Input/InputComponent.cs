
using UnityEngine;
using Game.Generic;

public class InputComponent
{

    public Vector3 moveAxis;
    public bool isPressShoot;
    public bool isPressJump;
    public bool isPressSwitchView;
    public bool isPressPickUpItem;
    public Vector3 grenadeThrowPoint;   //obsolete
    public Vector3 hookPoint; //obsolete

    public InputComponent()
    {
        moveAxis = new Vector3();
        grenadeThrowPoint = new Vector3();
        grenadeThrowPoint = new Vector3();
    }

    public void Reset()
    {
        moveAxis.Reset();
        grenadeThrowPoint.Reset();
        hookPoint.Reset();

        isPressShoot = false;
        isPressSwitchView = false;
        isPressJump = false;
        isPressPickUpItem = false;
    }

}
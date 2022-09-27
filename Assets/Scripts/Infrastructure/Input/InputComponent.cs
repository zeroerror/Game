
using UnityEngine;
using Game.Generic;

public class InputComponent
{

    public Vector3 moveAxis;
    public bool isPressFire;
    public bool isPressJump;
    public bool isPressSwitchView;
    public bool isPressPick;
    public bool isPressWeaponReload;
    public bool isPressDropWeapon;
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

        isPressFire = false;
        isPressSwitchView = false;
        isPressJump = false;
        isPressPick = false;
        isPressWeaponReload = false;
        isPressDropWeapon = false;
    }

}

using UnityEngine;

/// <summary>
/// 玩家游戏输入设置
/// </summary>
public static class InputGameSet
{

    public static PCInputSet PCInputSet { get; private set; }

    public static MobileInputSet MobileInputSet { get; private set; }

    public static void Ctor()
    {
        PCInputSet = new PCInputSet();
        MobileInputSet = new MobileInputSet();
        Debug.Log($"输入控制初始化完毕**************************************************************");
    }

    public static void TearDown()
    {
        PCInputSet = null;
        MobileInputSet = null;
    }

    public static void Receive_Input(ref InputComponent inputComponent)
    {
#if UNITY_STANDALONE
        ReceivePCInput(ref inputComponent);
#endif

#if UNITY_ANDROID
        ReceiveMobileInput(ref inputComponent);
#endif
    }

    public static void ReceivePCInput(ref InputComponent inputComponent)
    {
        if (Input.GetKey(PCInputSet.forward))
        {
            inputComponent.moveAxis.z = 1;
        }
        if (Input.GetKey(PCInputSet.backward))
        {
            inputComponent.moveAxis.z = -1;
        }
        if (Input.GetKey(PCInputSet.left))
        {
            inputComponent.moveAxis.x = -1;
        }
        if (Input.GetKey(PCInputSet.right))
        {
            inputComponent.moveAxis.x = 1;
        }
        if (Input.GetKeyDown(PCInputSet.swithView))
        {
            inputComponent.isPressSwitchView = true;
        }
        if (Input.GetKeyDown(PCInputSet.jump))
        {
            inputComponent.isPressJump = true;
        }
        if (Input.GetKeyDown(PCInputSet.shoot))
        {
            inputComponent.isPressShoot = true;
        }
        if (Input.GetKeyDown(PCInputSet.pickUpAssets))
        {
            inputComponent.isPressPickUpItem = true;
            Debug.Log("按下‘拾取’");
        }
        if (Input.GetKeyDown(PCInputSet.weaponReload))
        {
            inputComponent.isPressWeaponReload = true;
            Debug.Log("按下‘换弹’");
        }
        if (Input.GetKeyDown(PCInputSet.weaponReload))
        {
            inputComponent.isPressDropWeapon = true;
            Debug.Log("按下‘丢弃物器’");
        }
    }

    public static void ReceiveMobileInput(ref InputComponent inputComponent)
    {
        inputComponent.moveAxis = MobileInputSet.moveAixs;
        if (MobileInputSet.isPressJump)
        {
            inputComponent.isPressJump = true;
            MobileInputSet.isPressJump = false;
        }
        if (MobileInputSet.isPressShoot)
        {
            inputComponent.isPressShoot = true;
            MobileInputSet.isPressShoot = false;
        }
        if (MobileInputSet.isPressSwichingView)
        {
            inputComponent.isPressSwitchView = true;
            MobileInputSet.isPressSwichingView = false;
        }

    }

}

using UnityEngine;

/// <summary>
/// 玩家游戏输入设置
/// </summary>
public class PCInputSet
{

    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode swithView;
    public KeyCode shoot;
    public KeyCode pickUpAssets;

    public PCInputSet()
    {
        forward = KeyCode.W;
        backward = KeyCode.S;
        left = KeyCode.A;
        right = KeyCode.D;
        jump = KeyCode.Space;
        swithView = KeyCode.V;
        shoot = KeyCode.Mouse0;
        pickUpAssets = KeyCode.F;
    }

}
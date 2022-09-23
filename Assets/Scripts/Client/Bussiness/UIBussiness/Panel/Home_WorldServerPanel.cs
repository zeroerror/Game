using UnityEngine;
using ZeroUIFrame;
using Game.Client.Bussiness.EventCenter;

namespace Game.Bussiness.UIBussiness.Panel
{

    public class Home_WorldServerPanel : UIBehavior
    {

        void Awake()
        {
            var worldSerHosts = args[0] as string[];
            var ports = args[1] as ushort[];
            for (int i = 0; i < worldSerHosts.Length; i++)
            {
                Debug.Log($"{worldSerHosts[i]}:{ports[i]}");
                // TryAddChildUI("Home_WorldServerPanel","WorldServerPanel_Item");
            }
        }

        // == UI Click ==
    }

}
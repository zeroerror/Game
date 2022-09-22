using UnityEngine;
using UnityEngine.UI;
using ZeroUIFrame;
using Game.Bussiness.UIBussiness;

namespace Game.Bussiness.UIBussiness.Panel
{

    public class Home_WorldServerPanel : UIBehavior
    {

        UIEventCenter uIEventCenter;


        void Awake()
        {
            uIEventCenter = args[0] as UIEventCenter;
            Debug.Assert(uIEventCenter != null);

            object[] argss = (object[])args[1];
            var worldSerHosts = argss[0] as string[];
            var ports = argss[1] as ushort[];
            for (int i = 0; i < worldSerHosts.Length; i++)
            {
                Debug.Log($"{worldSerHosts[i]}:{ports[i]}");
                // TryAddChildUI("Home_WorldServerPanel","WorldServerPanel_Item");
            }
        }

        // == UI Click ==
    }

}
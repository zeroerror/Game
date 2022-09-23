using UnityEngine;
using ZeroUIFrame;
using Game.Client.Bussiness.EventCenter;

namespace Game.Bussiness.UIBussiness.Panel
{

    public class Home_WorldServerPanel : UIBehavior
    {

        string[] worldSerHosts;
        ushort[] ports;

        void Awake()
        {
            SetOnClick("SerGroup/Viewport/Content/Ser1", ClickConWorldSer, 0);
        }

        void OnEnable()
        {
            worldSerHosts = args[0] as string[];
            ports = args[1] as ushort[];
        }

        // == UI Click ==
        void ClickConWorldSer(params object[] args)
        {
            int index = (int)args[0];
            var host = worldSerHosts[index];
            var port = ports[index];
            UIEventCenter.ConnWorSerAction.Invoke(host, port);
        }

        }

    }
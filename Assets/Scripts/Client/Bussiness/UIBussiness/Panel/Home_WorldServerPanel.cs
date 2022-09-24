using UnityEngine;
using ZeroUIFrame;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.UIBussiness;

namespace Game.Bussiness.UIBussiness.Panel
{

    public class Home_WorldServerPanel : UIBehavior
    {

        string[] worldSerHosts;
        ushort[] ports;

        void Awake()
        {

        }

        void OnEnable()
        {
            worldSerHosts = args[1] as string[];
            ports = args[2] as ushort[];
            for (int i = 0; i < worldSerHosts.Length; i++)
            {
                var host = worldSerHosts[i];
                var port = ports[i];
                var go = UIManager.GetUIAsset("serverItem");
                go = GameObject.Instantiate(go);
                go.transform.SetParent(transform.Find("SerGroup/Viewport/Content"));
                string name = $"serverItem{i}";
                go.transform.name = name;
                string path = $"SerGroup/Viewport/Content/" + name;
                Text_SetText(path + "/host", $"{host}:{port}");
                SetOnClick(path, ClickConWorldSer, i);
            }
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
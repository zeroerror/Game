using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.Client2World;
using System.Collections.Generic;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {

        Queue<WolrdEnterResMessage> worldEnterQueue;

        WorldFacades worldFacades;

        public WorldController()
        {
            NetworkEventCenter.RegistLoginSuccess(OnLoginSuccess);
            NetworkEventCenter.RegistConnWorSerSuccess(SendWorldEnterReq);
            UIEventCenter.ConnWorSerAction += SendConnWorSer;

            worldEnterQueue = new Queue<WolrdEnterResMessage>();

        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
            worldFacades.Network.WorldReqAndRes.RegistRes_WorldEnter(OnEnterWorldRes);
        }

        public void Tick()
        {
            while (worldEnterQueue.TryDequeue(out var msg))
            {
                UIEventCenter.EnqueueTearDownQueue("Home_WorldServerPanel");
                var account = msg.account;
                Debug.Log($"account:{account}进入世界");
                SpawnScene("world_scene");
            }
        }

        void OnLoginSuccess(string[] worldSerHosts, ushort[] ports)
        {
            object[] args = { worldSerHosts, ports };
            UIEventCenter.EnqueueOpenQueue(new OpenEventModel { uiName = "Home_WorldServerPanel", args = args });
            SpawnScene("world_choose_scene");
        }

        // ====== SEND ======
        void SendConnWorSer(string host, ushort port)
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.ConnWorldServer(host, port);
        }

        void SendWorldEnterReq()
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.SendReq_WorldEnterMsg("sadasfsaf");
        }

        // ====== Response ======
        void OnEnterWorldRes(WolrdEnterResMessage msg)
        {
            worldEnterQueue.Enqueue(msg);
        }

        #region [Private Func]
        public async void SpawnScene(string sceneName)
        {
            Debug.Log($"开始加载世界:{sceneName}");
            var result = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single).Task;
            Debug.Log($"加载世界完成:{sceneName}");
            return;
        }
        #endregion

    }

}
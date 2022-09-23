using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.Client2World;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class WorldController
    {

        WorldFacades worldFacades;

        public WorldController()
        {
            NetworkEventCenter.RegistLoginSuccess(OnLoginSuccess);
            UIEventCenter.ConnWorSerAction += SendConnWorSer;
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
            worldFacades.Network.WorldReqAndRes.RegistRes_WorldEnter(OnConnWorSerSuccess);
        }

        public void Tick()
        {

        }

        async void OnLoginSuccess(string[] worldSerHosts, ushort[] ports)
        {
            object[] args = { worldSerHosts, ports };
            UIEventCenter.EnqueueOpenQueue(new OpenEventModel { uiName = "Home_WorldServerPanel", args = args });
            await SpawnScene("world_choose_scene");
        }

        async void OnConnWorSerSuccess(WolrdEnterResMessage msg)
        {
            var account = msg.account;
            NetworkEventCenter.Invoke_ConnWorSerSuccessHandler(msg.account);

            UIEventCenter.EnqueueTearDownQueue("Home_WorldServerPanel");
            await SpawnScene("world_scene");
        }

        void SendConnWorSer(string host, ushort port)
        {
            worldFacades.Network.WorldReqAndRes.ConnWorldServer(host, port);
        }

        public async Task<FieldEntity> SpawnScene(string sceneName)
        {
            var result = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single).Task;
            var scene = result.Scene;
            var sceneObjs = scene.GetRootGameObjects();
            var fieldTrans = sceneObjs[0].transform;
            var fieldEntity = fieldTrans.GetComponent<FieldEntity>();
            return fieldEntity;
        }

    }

}
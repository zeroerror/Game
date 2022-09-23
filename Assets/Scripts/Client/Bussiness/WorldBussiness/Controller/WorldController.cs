using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class WorldController
    {


        public WorldController()
        {
            NetworkEventCenter.RegistLoginSuccess(SpawnWorldServerChooseScene);
        }

        public void Inject(BattleFacades battleFacades)
        {

        }

        public void Tick()
        {

        }

        async void SpawnWorldServerChooseScene(string[] worldSerHosts, ushort[] ports)
        {
            var fieldEntity = await SpawnScene("world_server_choose_scene");
            object[] args = { worldSerHosts, ports };
            UIEventCenter.EnqueueOpenQueue(new OpenEventModel { uiName = "Home_WorldServerPanel", args = args });
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
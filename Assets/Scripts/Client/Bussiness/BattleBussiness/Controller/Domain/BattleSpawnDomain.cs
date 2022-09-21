using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Infrastructure.Input;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.BattleBussiness.Facades;
using System.Threading.Tasks;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleSpawnDomain
    {
        BattleFacades battleFacades;

        public BattleSpawnDomain()
        {
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public async Task<FieldEntity> SpawnBattleChooseScene() => await SpawnScene("BattleChooseScene");

        public async Task<FieldEntity> SpawnCityScene() => await SpawnScene("scene_city");

        public async Task<FieldEntity> SpawnWorldServerChooseScene() => await SpawnScene("world_server_choose_scene");

        public async Task<FieldEntity> SpawnScene(string sceneName)
        {
            var result = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single).Task;
            var scene = result.Scene;
            var sceneObjs = scene.GetRootGameObjects();
            var fieldTrans = sceneObjs[0].transform;
            var fieldEntity = fieldTrans.GetComponent<FieldEntity>();
            var cinemachineExtra = fieldTrans.GetComponentInChildren<CinemachineExtra>();

            var cameraAsset = battleFacades.Assets.CameraAsset;
            cameraAsset.TryGetByName("FirstViewCam", out GameObject firstViewCamPrefab);
            cameraAsset.TryGetByName("ThirdViewCam", out GameObject thirdViewCamPrefab);
            var firstCam = GameObject.Instantiate(firstViewCamPrefab).GetComponent<CinemachineExtra>();
            var thirdCam = GameObject.Instantiate(thirdViewCamPrefab).GetComponent<CinemachineExtra>();
            fieldEntity.CameraComponent.SetFirstViewCam(firstCam);
            fieldEntity.CameraComponent.SetThirdViewCam(thirdCam);

            LocalEventCenter.Invoke_SceneLoadedHandler(scene.name);
            return fieldEntity;
        }

    }

}
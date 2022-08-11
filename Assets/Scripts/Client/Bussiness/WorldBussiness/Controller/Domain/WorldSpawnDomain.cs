using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Infrastructure.Input;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.WorldBussiness.Facades;


namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class WorldSpawnDomain
    {
        WorldFacades worldFacades;

        public WorldSpawnDomain()
        {
            NetworkEventCenter.RegistLoginSuccess(EnterWorldChooseScene);
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
        }

        async void EnterWorldChooseScene()
        {
            var result = await Addressables.LoadSceneAsync("WorldChooseScene", LoadSceneMode.Single).Task;
            var scene = result.Scene;
            var sceneObjs = scene.GetRootGameObjects();
            var cinemachineExtra = sceneObjs[0].transform.GetComponentInChildren<CinemachineExtra>();

            Debug.Assert(cinemachineExtra != null, "cinemachineExtra 找不到");
            worldFacades.SetCinemachineExtra(cinemachineExtra);

            if (worldFacades.Assets.WorldRoleAssets.TryGetByName("player", out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, sceneObjs[0].transform);
            }

            Debug.Log("进入选择世界场景");
            LocalEventCenter.Invoke_SceneLoadedHandler(scene.name);
        }

    }

}
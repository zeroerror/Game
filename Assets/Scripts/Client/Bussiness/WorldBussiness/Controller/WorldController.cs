using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class WorldController
    {


        public WorldController()
        {

        }

        public void Inject(BattleFacades battleFacades)
        {

        }

        public void Tick()
        {

        }

        async void EnterWorldServerChooseScene(string[] worldSerHosts, ushort[] ports)
        {
            // // 当前有加载好的场景，则不加载
            // var curFieldEntity = battleFacades.Repo.FiledRepo.CurFieldEntity;
            // if (curFieldEntity != null) return;

            // // Load Scene And Spawn Field
            // var domain = battleFacades.Domain;
            // var fieldEntity = await domain.BattleSpawnDomain.SpawnCityScene();
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = true;
            // fieldEntity.SetFieldId(1);
            // var fieldEntityRepo = battleFacades.Repo.FiledRepo;
            // var physicsScene = fieldEntity.gameObject.scene.GetPhysicsScene();
            // fieldEntityRepo.Add(fieldEntity);
            // fieldEntityRepo.SetPhysicsScene(physicsScene);
            // // Send Spawn Role Message
            // var rqs = battleFacades.Network.BattleRoleReqAndRes;
            // rqs.SendReq_WolrdRoleSpawn();
            var fieldEntity = await SpawnScene("world_server_choose_scene");
            for (int i = 0; i < worldSerHosts.Length; i++)
            {
                Debug.Log($"世界服 {i} Host:{worldSerHosts[i]}  Port:{ports[i]}");
            }
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
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Infrastructure.Input;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleFieldDomain
    {

        BattleFacades battleFacades;

        public BattleFieldDomain() { }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public async void SpawBattleField(string fieldName)
        {
            // TODO: By fieldName
            var field = await SpawnField("scene_arena");
            field.SetEntityId(1);
            var fieldRepo = battleFacades.Repo.FieldRepo;
            fieldRepo.Add(field);
            fieldRepo.SetCurPhysicsScene(field.GetPhysicsScene());
        }

        public void RandomSpawnAllItemToField(FieldEntity fieldEntity, out List<EntityType> entityTypeList, out List<byte> subTypeList, out List<int> entityIDList)
        {
            GenerateRandomItemDataFromField(fieldEntity, out entityTypeList, out subTypeList);
            SpawnAllItemToField(fieldEntity, entityTypeList, subTypeList, out entityIDList);
        }

        public void SpawnAllItemToField(FieldEntity field, List<EntityType> entityTypeList, List<byte> subTypeList, out List<int> entityIDList)
        {
            entityIDList = new List<int>();
            AssetPointEntity[] assetPointEntities = field.transform.GetComponentsInChildren<AssetPointEntity>();
            int count = entityTypeList.Count;
            Debug.Log($"field {field.EntityId} 物件资源开始生成[数量:{count}]----------------------------------------------------");
            for (int i = 0; i < entityTypeList.Count; i++)
            {
                var entityType = entityTypeList[i];
                var subtype = subTypeList[i];
                var parentTF = assetPointEntities[i].transform;
                var spawnPos = parentTF.position;

                // - 获取资源ID
                var idService = battleFacades.IDService;
                var entityID = idService.GetAutoIDByEntityType(entityType);
                entityIDList.Add(entityID);

                // - 生成资源
                var commonDomain = battleFacades.Domain.commonDomain;
                commonDomain.SpawnEntity_Logic(entityType, subtype, entityID, spawnPos);
            }

            Debug.Log($"field {field.EntityId} 物件资源生成完毕******************************************************");
        }

        public void GenerateRandomItemDataFromField(FieldEntity fieldEntity, out List<EntityType> entityTypeList, out List<byte> subTypeList)
        {
            entityTypeList = new List<EntityType>();
            subTypeList = new List<byte>();

            AssetPointEntity[] assetPointEntities = fieldEntity.transform.GetComponentsInChildren<AssetPointEntity>();
            for (int i = 0; i < assetPointEntities.Length; i++)
            {
                var assetPoint = assetPointEntities[i];
                BattleAssetGenProbability[] probabilities = assetPoint.itemGenProbabilityArray;
                float totalWeight = 0;
                for (int j = 0; j < probabilities.Length; j++) totalWeight += probabilities[j].weight;
                float lRange = 0;
                float rRange = 0;
                float randomNumber = Random.Range(0f, 1f);
                for (int j = 0; j < probabilities.Length; j++)
                {
                    BattleAssetGenProbability probability = probabilities[j];
                    if (probability.weight <= 0) continue;
                    rRange = lRange + probability.weight / totalWeight;
                    if (randomNumber >= lRange && randomNumber < rRange)
                    {
                        entityTypeList.Add(probability.entityType);
                        subTypeList.Add(probability.subType);
                        break;
                    }
                    lRange = rRange;
                }
            }
        }

        async Task<FieldEntity> SpawnField(string sceneName)
        {
            var result = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single).Task;

            var scene = result.Scene;
            var sceneObjs = scene.GetRootGameObjects();
            var fieldTF = sceneObjs[0].transform;
            var field = fieldTF.GetComponent<FieldEntity>();
            field.Ctor();

            var cameraAsset = battleFacades.Assets.CameraAsset;
            cameraAsset.TryGetByName("FirstViewCam", out GameObject firstViewCamPrefab);
            cameraAsset.TryGetByName("ThirdViewCam", out GameObject thirdViewCamPrefab);
            var firstCam = GameObject.Instantiate(firstViewCamPrefab).GetComponent<CinemachineExtra>();
            var thirdCam = GameObject.Instantiate(thirdViewCamPrefab).GetComponent<CinemachineExtra>();
            field.CameraComponent.SetFirstViewCam(firstCam);
            field.CameraComponent.SetThirdViewCam(thirdCam);

            // LocalEventCenter.Invoke_SceneLoadedHandler(scene.name);
            return field;
        }

    }

}

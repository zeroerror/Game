using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleFieldDomain
    {

        BattleFacades battleFacades;

        byte tempRidIndex;

        public BattleFieldDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public async void SpawBattleField(string fieldName)
        {
            // TODO: By fieldName
            var domain = battleFacades.Domain;
            var fieldEntity = await domain.SceneDomain.SpawnGameFightScene();
            fieldEntity.SetEntityId(1);
            var fieldEntityRepo = battleFacades.Repo.FiledRepo;
            fieldEntityRepo.Add(fieldEntity);
            fieldEntityRepo.SetPhysicsScene(fieldEntity.gameObject.scene.GetPhysicsScene());
        }

        public void SpawnAllItemToField(FieldEntity field, List<EntityType> entityTypeList, List<byte> subTypeList, out List<int> entityIDList)
        {
            entityIDList = new List<int>();
            AssetPointEntity[] assetPointEntities = field.transform.GetComponentsInChildren<AssetPointEntity>();
            int count = entityTypeList.Count;
            Debug.Log($"物件资源开始生成[数量:{count}]----------------------------------------------------");
            for (int i = 0; i < entityTypeList.Count; i++)
            {
                var entityType = entityTypeList[i];
                var subtype = subTypeList[i];
                var parent = assetPointEntities[i];

                // - 获取资源ID
                var idService = battleFacades.IDService;
                var entityID = idService.GetAutoIDByEntityType(entityType);
                entityIDList.Add(entityID);

                // - 生成资源
                var itemDomain = battleFacades.Domain.ItemDomain;
                itemDomain.SpawnItem(entityType, subtype, entityID, parent.transform);

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
                AssetGenProbability[] itemGenProbabilities = assetPoint.itemGenProbabilityArray;
                float totalWeight = 0;
                for (int j = 0; j < itemGenProbabilities.Length; j++) totalWeight += itemGenProbabilities[j].weight;
                float lRange = 0;
                float rRange = 0;
                float randomNumber = Random.Range(0f, 1f);
                for (int j = 0; j < itemGenProbabilities.Length; j++)
                {
                    AssetGenProbability igp = itemGenProbabilities[j];
                    if (igp.weight <= 0) continue;
                    rRange = lRange + igp.weight / totalWeight;
                    if (randomNumber >= lRange && randomNumber < rRange)
                    {
                        entityTypeList.Add(igp.entityType);
                        subTypeList.Add(igp.subType);
                        break;
                    }
                    lRange = rRange;
                }
            }
        }

    }

}

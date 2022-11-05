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

        public async void SpawBattleScene()
        {
            // Load Scene And Spawn Field
            var domain = battleFacades.Domain;
            var fieldEntity = await domain.SceneDomain.SpawnGameFightScene();
            fieldEntity.SetEntityId(1);
            var fieldEntityRepo = battleFacades.Repo.FiledRepo;
            fieldEntityRepo.Add(fieldEntity);
            fieldEntityRepo.SetPhysicsScene(fieldEntity.gameObject.scene.GetPhysicsScene());

            // 生成资源
            GenerateRandomAssetData(fieldEntity, out var assetPointEntities);
            InitAllAssetRepo(assetPointEntities);
        }

        void InitAllAssetRepo(AssetPointEntity[] assetPointEntities)
        {
            var entityTypeList = battleFacades.EntityTypeList;
            var entityIDList = battleFacades.EntityIDList;
            var itemTypeByteList = battleFacades.ItemTypeByteList;
            var subTypeList = battleFacades.SubTypeList;

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

                // - 生成资源
                var itemDomain = battleFacades.Domain.ItemDomain;
                itemDomain.SpawnItem(entityType, subtype, entityID, parent.transform);

                // - 记录
                entityIDList.Add(entityID);
                itemTypeByteList.Add((byte)entityType);
            }

            Debug.Log($"物件资源生成完毕******************************************************");
        }

        void GenerateRandomAssetData(FieldEntity fieldEntity, out AssetPointEntity[] assetPointEntities)
        {
            assetPointEntities = fieldEntity.transform.GetComponentsInChildren<AssetPointEntity>();
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
                        battleFacades.EntityTypeList.Add(igp.entityType);
                        battleFacades.SubTypeList.Add(igp.subType);
                        break;
                    }
                    lRange = rRange;
                }
            }
        }
    }

}

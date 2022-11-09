using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleEvolveItemDomain
    {

        BattleFacades battleFacades;

        public BattleEvolveItemDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleEvolveItemEntity Spawn(byte subType, int entityID, Vector3 pos)
        {
            var prefabName = $"Item_Evolve_{(1000 + subType).ToString()}";
            var assets = battleFacades.Assets.ItemAsset;
            if (!assets.TryGetByName(prefabName, out var prefab))
            {
                Debug.LogError($"{prefabName} 生成失败");
                return null;
            }

            var go = GameObject.Instantiate(prefab);
            var entity = go.GetComponent<BattleEvolveItemEntity>();
            entity.Ctor();
            entity.SetEntityID(entityID);
            entity.transform.position = pos;

            var repo = battleFacades.Repo;
            var evolveItemRepo = repo.EvolveItemRepo;
            evolveItemRepo.Add(entity);

            return entity;
        }

        public void TearDownEvolveItem(BattleEvolveItemEntity evolveItem)
        {
            var repo = battleFacades.Repo;
            var armorEvolveItemRepo = repo.EvolveItemRepo;
            armorEvolveItemRepo.TryRemove(evolveItem);
            GameObject.Destroy(evolveItem.gameObject);
            GameObject.Destroy(evolveItem);
        }

    }

}
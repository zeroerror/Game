using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleArmorItemDomain
    {

        BattleFacades battleFacades;

        public BattleArmorItemDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleArmorItemEntity Spawn(ArmorType armorType, int entityID, Vector3 pos)
        {
            string prefabName = $"Item_Armor_{armorType.ToString()}";

            var asset = battleFacades.Assets.ItemAsset;
            if (!asset.TryGetByName(prefabName, out var prefab))
            {
                Debug.LogError($"{prefabName} Spawn Failed!");
                return null;
            }
            
            var go = GameObject.Instantiate(prefab);
            var entity = go.GetComponent<BattleArmorItemEntity>();
            entity.Ctor();
            entity.SetEntityID(entityID);
            entity.transform.position = pos;

            var repo = battleFacades.Repo;
            var armorItemRepo = repo.ArmorItemRepo;
            armorItemRepo.Add(entity);

            return entity;

        }

        public void TearDownArmorItem(BattleArmorItemEntity armorItem)
        {
            var repo = battleFacades.Repo;
            var armorItemRepo = repo.ArmorItemRepo;
            armorItemRepo.TryRemove(armorItem);
            GameObject.Destroy(armorItem.gameObject);
            GameObject.Destroy(armorItem);
        }

    }

}
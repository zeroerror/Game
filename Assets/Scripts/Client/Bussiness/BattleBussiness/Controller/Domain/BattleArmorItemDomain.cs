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

        public BattleArmorItemEntity SpawnBattleArmorItem(GameObject entityGo, int entityID)
        {
            var armorItem = entityGo.GetComponent<BattleArmorItemEntity>();
            armorItem.Ctor();
            armorItem.SetEntityID(entityID);

            var repo = battleFacades.Repo;
            var armorItemRepo = repo.ArmorItemRepo;
            armorItemRepo.Add(armorItem);

            return armorItem;
        }

        public BattleArmorItemEntity SpawnBattleArmorItem(ArmorType armorType, int entityID)
        {
            string prefabName = $"Item_Armor_{armorType.ToString()}";

            var asset = battleFacades.Assets.ItemAsset;
            if (asset.TryGetByName(prefabName, out var prefab))
            {
                var go = GameObject.Instantiate(prefab);
                var armorItem = go.GetComponent<BattleArmorItemEntity>();
                armorItem.SetEntityID(entityID);

                var repo = battleFacades.Repo;
                var armorItemRepo = repo.ArmorItemRepo;
                armorItemRepo.Add(armorItem);

                return armorItem;
            }

            return null;
        }

        public void TearDownWeaponItem(BattleArmorItemEntity weaponItem)
        {
            var repo = battleFacades.Repo;
            var armorItemRepo = repo.ArmorItemRepo;
            armorItemRepo.TryRemove(weaponItem);
            GameObject.Destroy(weaponItem.gameObject);
            GameObject.Destroy(weaponItem);
        }

    }

}
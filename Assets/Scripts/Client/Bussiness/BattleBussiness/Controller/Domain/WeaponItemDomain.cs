using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class WeaponItemDomain
    {

        BattleFacades battleFacades;

        public WeaponItemDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public WeaponItemEntity SpawnWeaponItem(GameObject entityGo, int entityID)
        {
            var weaponItem = entityGo.GetComponent<WeaponItemEntity>();
            weaponItem.Ctor();

            var repo = battleFacades.Repo;
            var weaponItemRepo = repo.WeaponItemRepo;
            weaponItem.SetEntityID(entityID);
            weaponItemRepo.Add(weaponItem);

            return weaponItem;
        }

        public WeaponItemEntity SpawnWeaponItem(WeaponType weaponType, int entityID)
        {
            string prefabName = $"Item_Weapon_{weaponType.ToString()}";

            var asset = battleFacades.Assets.ItemAsset;
            if (asset.TryGetByName(prefabName, out var prefab))
            {
                var go = GameObject.Instantiate(prefab);
                var weaponItem = go.GetComponent<WeaponItemEntity>();
                weaponItem.Ctor();
                weaponItem.SetEntityID(entityID);

                var repo = battleFacades.Repo;
                var weaponItemRepo = repo.WeaponItemRepo;
                weaponItemRepo.Add(weaponItem);
                return weaponItem;
            }

            return null;
        }

        public void TearDownWeaponItem(WeaponItemEntity weaponItem)
        {
            var repo = battleFacades.Repo;
            var weaponItemRepo = repo.WeaponItemRepo;
            weaponItemRepo.TryRemove(weaponItem);
            GameObject.Destroy(weaponItem.gameObject);
            GameObject.Destroy(weaponItem);
        }

    }

}
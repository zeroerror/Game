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

        public WeaponItemEntity Spawn(WeaponType weaponType, int entityID, Vector3 pos)
        {
            string prefabName = $"Item_Weapon_{weaponType.ToString()}";
            var asset = battleFacades.Assets.ItemAsset;
            if (!asset.TryGetByName(prefabName, out var prefab))
            {
                Debug.LogError($"{prefabName} Spawn Failed!");
                return null;
            }

            var go = GameObject.Instantiate(prefab);
            var entity = go.GetComponent<WeaponItemEntity>();
            entity.Ctor();
            entity.SetEntityID(entityID);
            entity.transform.position = pos;

            var repo = battleFacades.Repo;
            var weaponItemRepo = repo.WeaponItemRepo;
            weaponItemRepo.Add(entity);
            return entity;
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
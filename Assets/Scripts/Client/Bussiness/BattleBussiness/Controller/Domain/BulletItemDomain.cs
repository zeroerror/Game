using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BulletItemDomain
    {

        BattleFacades battleFacades;

        public BulletItemDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BulletItemEntity SpawnBulletItem(GameObject entityGo, int entityID)
        {
            var bulletItem = entityGo.GetComponent<BulletItemEntity>();
            bulletItem.Ctor();

            var repo = battleFacades.Repo;
            var bulletItemRepo = repo.BulletItemRepo;
            bulletItem.SetEntityID(entityID);
            bulletItemRepo.Add(bulletItem);
            
            return bulletItem;
        }


        public BulletItemEntity SpawnBulletItem(WeaponType weaponType, int entityID)
        {
            string prefabName = $"Item_Weapon_{weaponType.ToString()}";

            var asset = battleFacades.Assets.ItemAsset;
            if (asset.TryGetByName(prefabName, out var prefab))
            {
                var go = GameObject.Instantiate(prefab);
                var weapon = go.GetComponent<BulletItemEntity>();
                var repo = battleFacades.Repo;
                var bulletItemRepo = repo.BulletItemRepo;
                weapon.SetEntityID(entityID);
                bulletItemRepo.Add(weapon);
                return weapon;
            }

            return null;
        }

        public void TearDownBulletItem(BulletItemEntity bulletItem)
        {
            var repo = battleFacades.Repo;
            var bulletItemRepo = repo.BulletItemRepo;
            bulletItemRepo.TryRemove(bulletItem);
            GameObject.Destroy(bulletItem.gameObject);
            GameObject.Destroy(bulletItem);
        }

    }

}
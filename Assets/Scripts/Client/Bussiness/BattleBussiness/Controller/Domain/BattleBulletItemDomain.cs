using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleBulletItemDomain
    {

        BattleFacades battleFacades;

        public BattleBulletItemDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BulletItemEntity Spawn(BulletType bulletType, int entityID, Vector3 pos)
        {
            string prefabName = $"Item_Bullet_{bulletType.ToString()}";
            var asset = battleFacades.Assets.ItemAsset;
            if (!asset.TryGetByName(prefabName, out var prefab))
            {
                Debug.LogError($"{prefabName} Spawn Failed!");
                return null;
            }

            var go = GameObject.Instantiate(prefab);
            var entity = go.GetComponent<BulletItemEntity>();
            entity.Ctor();
            entity.SetEntityID(entityID);
            entity.transform.position = pos;

            var repo = battleFacades.Repo;
            var bulletItemRepo = repo.BulletItemRepo;
            bulletItemRepo.Add(entity);
            return entity;
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
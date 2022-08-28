using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Network;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class BulletSpawnDomain
    {

        WorldFacades worldFacades;

        public BulletSpawnDomain()
        {
        }

        public void Inject(WorldFacades facades)
        {
            this.worldFacades = facades;
        }

        public BulletEntity SpawnBullet(Transform parent)
        {
            string bulletPrefabName = "Bullet";
            Debug.Log("生成" + bulletPrefabName);
            if (worldFacades.Assets.BulletAsset.TryGetByName(bulletPrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var entity = prefabAsset.GetComponent<BulletEntity>();
                return entity;
            }

            return null;

        }

    }

}
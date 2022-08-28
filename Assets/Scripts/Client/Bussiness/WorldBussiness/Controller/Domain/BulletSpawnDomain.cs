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

        public BulletEntity SpawnBullet(Transform parent, BulletType bulletType)
        {
            string bulletPrefabName = "Bullet";
            switch (bulletType)
            {
                case BulletType.Default:
                    bulletPrefabName = "Bullet";
                    break;
                case BulletType.Grenade:
                    bulletPrefabName = "Grenade";
                    break;
            }
            if (worldFacades.Assets.BulletAsset.TryGetByName(bulletPrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var bulletEntity = prefabAsset.GetComponent<BulletEntity>();
                if (bulletType == BulletType.Grenade)
                {
                    var grenadeEntity = (GrenadeEntity)bulletEntity;
                    grenadeEntity.SetLifeTime(3f); //手榴弹生命周期
                    grenadeEntity.SetMoveComponent(10f);//手榴弹投掷速度
                }

                bulletEntity.SetBulletType(bulletType);
                return bulletEntity;
            }

            return null;
        }

        void Spawn()
        {

        }

    }

}
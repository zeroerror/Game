using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BulletDomain
    {

        BattleFacades battleFacades;

        public BulletDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BulletEntity SpawnBullet(Transform parent, BulletType bulletType)
        {
            string bulletPrefabName = bulletType.ToString();

            if (battleFacades.Assets.BulletAsset.TryGetByName(bulletPrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var bulletEntity = prefabAsset.GetComponent<BulletEntity>();
                bulletEntity.SetBulletType(bulletType);
                bulletEntity.Ctor();
                return bulletEntity;
            }

            return null;
        }

        public List<BulletEntity> Tick_BulletLife(float deltaTime)
        {
            var bulletRepo = battleFacades.Repo.BulletRepo;
            List<BulletEntity> removeList = new List<BulletEntity>();
            bulletRepo.Foreach((bulletEntity) =>
            {
                if (bulletEntity.LifeTime <= 0)
                {
                    removeList.Add(bulletEntity);

                }
                bulletEntity.ReduceLifeTime(deltaTime);
            });

            return removeList;
        }

        public void Tick_Bullet(float fixedDeltaTime)
        {
            var bulletRepo = battleFacades.Repo.BulletRepo;
            bulletRepo.Foreach((bullet) =>
            {
                switch (bullet.BulletType)
                {
                    case BulletType.DefaultBullet:
                        break;
                    case BulletType.Grenade:
                        break;
                    case BulletType.Hooker:
                        break;
                }

                bullet.MoveComponent.Tick_Friction(fixedDeltaTime);
                bullet.MoveComponent.Tick_Gravity(fixedDeltaTime);
                bullet.MoveComponent.Tick_Rigidbody(fixedDeltaTime);
            });
        }

        // == Hooker
        // 获得当前所有已激活的爪钩
        public List<HookerEntity> GetActiveHookerList()
        {
            List<HookerEntity> hookerEntities = new List<HookerEntity>();
            var bulletRepo = battleFacades.Repo.BulletRepo;
            bulletRepo.Foreach((bullet) =>
            {
                if (bullet is HookerEntity hookerEntity && hookerEntity.GrabPoint != null)
                {
                    hookerEntities.Add(hookerEntity);
                }
            });

            return hookerEntities;
        }

    }

}
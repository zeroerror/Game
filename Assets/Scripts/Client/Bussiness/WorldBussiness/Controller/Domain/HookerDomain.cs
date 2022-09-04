using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Infrastructure.Generic;

namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class HookerDomain
    {

        WorldFacades worldFacades;

        public HookerDomain()
        {
        }

        public void Inject(WorldFacades facades)
        {
            this.worldFacades = facades;
        }

        public HookerEntity SpawnHooker(Transform master)
        {
            string bulletPrefabName = "Hooker";
         
            if (worldFacades.Assets.BulletAsset.TryGetByName(bulletPrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, master);
                var bulletEntity = prefabAsset.GetComponent<BulletEntity>();
                bulletEntity.SetBulletType(BulletType.Hooker);
                return (HookerEntity)bulletEntity;
            }

            return null;
        }

        public List<HookerEntity> Tick_HookerLife(float deltaTime)
        {
            var bulletRepo = worldFacades.Repo.BulletEntityRepo;
            List<HookerEntity> removeList = new List<HookerEntity>();
            bulletRepo.Foreach((bulletEntity) =>
            {
                if (bulletEntity.LifeTime < 0)
                {
                    removeList.Add((HookerEntity)bulletEntity);
                }
                bulletEntity.ReduceLifeTime(deltaTime);
            });

            return removeList;
        }

        public void Tick_HookerMovement(float deltaTime)
        {
            var bulletRepo = worldFacades.Repo.BulletEntityRepo;
            bulletRepo.Foreach((bullet) =>
            {
                bullet.MoveComponent.Tick_Rigidbody(deltaTime);
            });
        }

    }

}
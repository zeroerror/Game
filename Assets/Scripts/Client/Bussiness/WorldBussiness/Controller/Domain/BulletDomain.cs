using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Infrastructure.Generic;

namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class BulletDomain
    {

        WorldFacades worldFacades;

        public BulletDomain()
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
                case BulletType.Hooker:
                    bulletPrefabName = "Hooker";
                    break;
            }

            if (worldFacades.Assets.BulletAsset.TryGetByName(bulletPrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var bulletEntity = prefabAsset.GetComponent<BulletEntity>();
                bulletEntity.SetBulletType(bulletType);
                return bulletEntity;
            }

            return null;
        }

        public List<BulletEntity> Tick_BulletLife(float deltaTime)
        {
            var bulletRepo = worldFacades.Repo.BulletEntityRepo;
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
            var bulletRepo = worldFacades.Repo.BulletEntityRepo;
            bulletRepo.Foreach((bullet) =>
            {
                switch (bullet.BulletType)
                {
                    case BulletType.Default:
                        break;
                    case BulletType.Grenade:
                        break;
                    case BulletType.Hooker:
                        var hookerEntity = (HookerEntity)bullet;
                        var master = hookerEntity.MasterEntity;
                        var masterMC = master.MoveComponent;
                        if (hookerEntity.TickHooker(out float force))
                        {
                            var hookerEntityMC = hookerEntity.MoveComponent;
                            var dir = hookerEntityMC.CurPos - masterMC.CurPos;
                            var dis = Vector3.Distance(hookerEntityMC.CurPos, masterMC.CurPos);
                            dir.Normalize();
                            var v = dir * force * fixedDeltaTime;
                            Debug.Log($"Hooker : v:{v} ");
                            masterMC.AddExtraVelocity(v);
                        }
                        break;
                }

                bullet.MoveComponent.Tick_Friction(fixedDeltaTime);
                bullet.MoveComponent.Tick_GravityVelocity(fixedDeltaTime);
                bullet.MoveComponent.Tick_Rigidbody(fixedDeltaTime);
            });
        }

    }

}
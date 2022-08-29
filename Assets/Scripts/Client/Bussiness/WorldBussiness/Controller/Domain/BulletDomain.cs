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

        public void Tick_BulletLife(float deltaTime)
        {
            var bulletRepo = worldFacades.Repo.BulletEntityRepo;
            List<BulletEntity> removeList = new List<BulletEntity>();
            bulletRepo.Foreach((bulletEntity) =>
            {
                if (bulletEntity.LifeTime < 0)
                {
                    removeList.Add(bulletEntity);

                }
                bulletEntity.ReduceLifeTime(deltaTime);
            });

            removeList.ForEach((bulletEntity) =>
            {
                if (bulletEntity.BulletType == BulletType.Default)
                {
                    bulletEntity.TearDown();
                }

                if (bulletEntity.BulletType == BulletType.Grenade)
                {
                    ((GrenadeEntity)bulletEntity).TearDown();
                    var roleRepo = worldFacades.Repo.WorldRoleRepo;
                    roleRepo.Foreach((role) =>
                    {
                        var dis = Vector3.Distance(role.MoveComponent.CurPos, bulletEntity.MoveComponent.CurPos);
                        if (dis < 5f)
                        {
                            var dir = role.MoveComponent.CurPos - bulletEntity.MoveComponent.CurPos;
                            dir.Normalize();
                            role.MoveComponent.AddVelocity(dir * 10f);
                        }
                    });
                }

                bulletRepo.TryRemove(bulletEntity);
            });
        }

       public void Tick_BulletMovement()
        {
            var bulletRepo = worldFacades.Repo.BulletEntityRepo;
            bulletRepo.Foreach((bullet) =>
            {
                bullet.MoveComponent.Tick(UnityEngine.Time.deltaTime);
            });
        }

    }

}
using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BulletRendererDomain
    {

        BattleFacades battleFacades;

        public BulletRendererDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public void Update_AllBulletRenderer(float deltaTime)
        {
            var repo = battleFacades.Repo;
            var bulletLogicRepo = repo.BulletLogicRepo;
            var bulletRendererRepo = repo.BulletRendererRepo;

            bulletLogicRepo.Foreach((bulletLogic) =>
            {
                var bulletRenderer = bulletRendererRepo.Get(bulletLogic.IDComponent.EntityID);
                bulletRenderer.SetPosition(Vector3.Lerp(bulletRenderer.transform.position, bulletLogic.Position, deltaTime * bulletRenderer.posAdjust));
                bulletRenderer.SetRotation(Quaternion.Lerp(bulletRenderer.transform.rotation, bulletLogic.Rotation, deltaTime * bulletRenderer.rotAdjust));
            });
        }

        public BulletRendererEntity SpawnBulletRenderer(BulletType bulletType, int entityID)
        {
            string bulletPrefabName = bulletType.ToString() + "_Renderer";

            if (battleFacades.Assets.BulletAsset.TryGetByName(bulletPrefabName, out GameObject prefabAsset))
            {
                var repo = battleFacades.Repo;
                var parent = repo.FieldRepo.CurFieldEntity.transform;
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);

                var bulletRenderer = prefabAsset.GetComponent<BulletRendererEntity>();
                bulletRenderer.Ctor();
                bulletRenderer.SetEntityID(entityID);

                repo.BulletRendererRepo.Add(bulletRenderer);
                return bulletRenderer;
            }

            return null;
        }

        public void TearDownBulletRenderer(BulletRendererEntity entity)
        {
            var repo = battleFacades.Repo;
            var bulletRendererRepo = repo.BulletRendererRepo;

            entity.TearDown();
            bulletRendererRepo.TryRemove(entity);
        }

        public void TearDownBulletRenderer(int entityID)
        {
            var repo = battleFacades.Repo;
            var bulletRendererRepo = repo.BulletRendererRepo;
            var entity = bulletRendererRepo.Get(entityID);

            entity.TearDown();
            bulletRendererRepo.TryRemove(entity);
        }

    }

}
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleCommonDomain
    {

        BattleFacades battleFacades;

        public BattleCommonDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public void ClearBattleField()
        {
            var repo = battleFacades.Repo;
            var domain = battleFacades.Domain;

            // - Role
            var roleDomain = battleFacades.Domain.RoleLogicDomain;
            var roleRepo = battleFacades.Repo.RoleLogicRepo;
            roleRepo.Foreach((role) =>
            {
                roleDomain.Reborn(role);
            });
            // - Field
            var fieldRepo = repo.FieldRepo;
            var curField = fieldRepo.CurFieldEntity;
            curField.ResetBornPointFlags();
            // - Item
            var itemDomain = domain.ItemDomain;
            itemDomain.TearDownAllItems();
            // - Bullet
            var bulletLogicRepo = repo.BulletLogicRepo;
            bulletLogicRepo.ForAll((bulletLogic) =>
            {
                var bulletDomain = domain.BulletLogicDomain;
                bulletDomain.TearDown(bulletLogic);
            });
            var bulletRendererRepo = repo.BulletRendererRepo;
            bulletRendererRepo.ForAll((bulletRenderer) =>
            {
                var bulletDomain = domain.BulletRendererDomain;
                bulletDomain.TearDownBulletRenderer(bulletRenderer);
            });
            // - Weapon
            var weaponRepo = repo.WeaponRepo;
            weaponRepo.ForAll((weapon) =>
            {
                var WeaponDomain = domain.WeaponDomain;
                WeaponDomain.TearDownWeapon(weapon);
            });

        }

        public void SpawnEntityLogicAndRenderer(Vector3 pos, EntityType entityType, byte subType, int entityID)
        {
            SpawnEntityLogic(pos, entityType, subType, entityID);
            SpawnEntityRenderer(pos, entityType, subType, entityID);
        }

        public void SpawnEntityLogic(Vector3 pos, EntityType entityType, byte subType, int entityID)
        {
            var domain = battleFacades.Domain;

            if (entityType == EntityType.Aridrop)
            {
                var airdropLogicDomain = domain.AirdropLogicDomain;
                var curLevelStage = battleFacades.GameEntity.Stage.GetCurLevelStage();
                airdropLogicDomain.SpawnLogic(entityID, curLevelStage, pos);
                return;
            }

            Debug.LogError("未处理");
        }

        public void SpawnEntityRenderer(Vector3 pos, EntityType entityType, byte subType, int entityID)
        {
            var domain = battleFacades.Domain;
            if (entityType == EntityType.Aridrop)
            {
                var airdropRendererDomain = domain.AirdropRendererDomain;
                var curLevelStage = battleFacades.GameEntity.Stage.GetCurLevelStage();
                airdropRendererDomain.SpawnRenderer(entityID, curLevelStage, pos);
                return;
            }

            Debug.LogError("未处理");
        }


        public void TearDownEntityLogicAndRenderer(Vector3 pos, EntityType entityType, int entityID)
        {
            TearDownEntityLogic(pos, entityType, entityID);
            TearDownEntityRenderer(pos, entityType, entityID);
        }

        public void TearDownEntityLogic(Vector3 pos, EntityType entityType, int entityID)
        {
            var repo = battleFacades.Repo;
            var domain = battleFacades.Domain;

            if (entityType == EntityType.Bullet)
            {
                var bulletLogicRepo = repo.BulletLogicRepo;
                var bulletLogic = bulletLogicRepo.Get(entityID);
                bulletLogic.LocomotionComponent.SetPosition(pos);
                var bulletLogicDomain = domain.BulletLogicDomain;
                bulletLogicDomain.TearDown(bulletLogic);
                return;
            }

            if (entityType == EntityType.Aridrop)
            {
                var airdropLogicRepo = repo.AirdropLogicRepo;
                var airdropLogic = airdropLogicRepo.Get(entityID);
                var airdropLogicDomain = domain.AirdropLogicDomain;
                airdropLogicDomain.TearDownLogic(airdropLogic);
                return;
            }

            Debug.LogError("未处理");
        }

        public void TearDownEntityRenderer(Vector3 pos, EntityType entityType, int entityID)
        {
            var repo = battleFacades.Repo;
            var domain = battleFacades.Domain;

            if (entityType == EntityType.Bullet)
            {
                var bulletRendererDomain = domain.BulletRendererDomain;
                bulletRendererDomain.TearDownBulletRenderer(entityID);
                return;
            }

            if (entityType == EntityType.Aridrop)
            {
                var airdropRendererDomain = domain.AirdropRendererDomain;
                airdropRendererDomain.TearDownRenderer(entityID);
                return;
            }

            Debug.LogError("未处理");
        }

    }

}
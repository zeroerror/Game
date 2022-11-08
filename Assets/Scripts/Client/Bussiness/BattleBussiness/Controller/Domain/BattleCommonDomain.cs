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
            var bulletLogicRepo = repo.BulletRepo;
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

        public void TearDownEntityLogicAndRenderer(Vector3 pos, EntityType entityType, int entityID)
        {

            var repo = battleFacades.Repo;
            var domain = battleFacades.Domain;

            if (entityType == EntityType.Bullet)
            {
                var bullerRepo = repo.BulletRepo;
                var bulletLogic = bullerRepo.Get(entityID);
                bulletLogic.LocomotionComponent.SetPosition(pos);
                var bulletLogicDomain = domain.BulletLogicDomain;
                bulletLogicDomain.TearDown(bulletLogic);
                var bulletRendererDomain = domain.BulletRendererDomain;
                bulletRendererDomain.TearDownBulletRenderer(bulletLogic.IDComponent.EntityID);
                return;
            }

            if (entityType == EntityType.Aridrop)
            {
                var airdropLogicRepo = repo.AirdropLogicRepo;
                var airdropLogic = airdropLogicRepo.Get(entityID);
                var airdropLogicDomain = domain.AirdropLogicDomain;
                airdropLogicDomain.TearDownLogic(airdropLogic);
                var airdropRendererDomain = domain.AirdropRendererDomain;
                airdropRendererDomain.TearDownRenderer(airdropLogic.IDComponent.EntityID);
                return;
            }

            Debug.LogError("未处理");
        }

    }

}
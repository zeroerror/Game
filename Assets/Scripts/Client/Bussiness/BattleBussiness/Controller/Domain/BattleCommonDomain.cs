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
            var roleDomain = battleFacades.Domain.RoleDomain;
            var roleRepo = battleFacades.Repo.RoleLogicRepo;
            roleRepo.Foreach((role) =>
            {
                roleDomain.Reborn(role);
            });
            // - Field
            var fieldRepo = repo.FieldRepo;
            var curField = fieldRepo.CurFieldEntity;
            curField.Reset();
            // - Item
            var itemDomain = domain.ItemDomain;
            itemDomain.TearDownAllItems();
            // - Bullet
            var bulletLogicRepo = repo.BulletRepo;
            bulletLogicRepo.ForAll((bulletLogic) =>
            {
                var bulletDomain = domain.BulletLogicDomain;
                bulletDomain.TearDownBulletLogic(bulletLogic);
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

    }

}
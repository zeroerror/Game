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
                bulletDomain.TearDown(bulletRenderer);
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
            SpawnEntity_Logic(entityType, subType, entityID, pos);
            SpawnEntity_Renderer(pos, entityType, subType, entityID);
        }

        public void SpawnEntity_Renderer(Vector3 pos, EntityType entityType, byte subType, int entityID)
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
            TearDownEntity_Logic(pos, entityType, entityID);
            TearDownEntity_Renderer(pos, entityType, entityID);
        }

        public void TearDownEntity_Logic(Vector3 pos, EntityType entityType, int entityID)
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

        public void TearDownEntity_Renderer(Vector3 pos, EntityType entityType, int entityID)
        {
            var repo = battleFacades.Repo;
            var domain = battleFacades.Domain;

            if (entityType == EntityType.Bullet)
            {
                var bulletRendererDomain = domain.BulletRendererDomain;
                bulletRendererDomain.TearDown(entityID);
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

        public object SpawnEntity_Logic(EntityType entityType, byte subType, int entityID, Vector3 pos)
        {
            var allDomains = battleFacades.Domain;
            if (entityType == EntityType.BattleRole)
            {
                var domain = allDomains.RoleLogicDomain;
                return domain.SpawnLogic(entityID);
            }
            if (entityType == EntityType.Weapon)
            {
                var domain = allDomains.WeaponDomain;
                var weaponType = (WeaponType)subType;
                return domain.Spawn(weaponType, entityID);
            }
            if (entityType == EntityType.Bullet)
            {
                var domain = allDomains.BulletLogicDomain;
                var bulletType = (BulletType)subType;
                return domain.SpawnLogic(bulletType, entityID, pos);
            }
            if (entityType == EntityType.WeaponItem)
            {
                var domain = allDomains.WeaponItemDomain;
                var weaponType = (WeaponType)subType;
                return domain.Spawn(weaponType, entityID, pos);
            }
            if (entityType == EntityType.BulletItem)
            {
                var domain = allDomains.BulletItemDomain;
                var bulletType = (BulletType)subType;
                return domain.Spawn(bulletType, entityID, pos);
            }
            if (entityType == EntityType.ArmorItem)
            {
                var domain = allDomains.ArmorItemDomain;
                var armorType = (ArmorType)subType;
                return domain.Spawn(armorType, entityID, pos);
            }
            if (entityType == EntityType.EvolveItem)
            {
                var domain = allDomains.EvolveItemDomain;
                return domain.Spawn(subType, entityID, pos);
            }
            if (entityType == EntityType.Aridrop)
            {
                var domain = allDomains.AirdropLogicDomain;
                var curLvStage = battleFacades.GameEntity.Stage.GetCurLevelStage(); // TODO: MAY CAUSE WRONG!
                return domain.SpawnLogic(curLvStage, entityID, pos);
            }

            Debug.LogError("Not Handler");
            return null;
        }

        public GameObject UnpackEntityObjToGO(object obj, EntityType entityType)
        {
            if (obj is BattleRoleLogicEntity roleLogic)
            {
                var go = roleLogic.gameObject;
                return go;
            }
            if (obj is BattleRoleRendererEntity roleRenderer)
            {
                var go = roleRenderer.gameObject;
                return go;
            }
            if (obj is WeaponEntity weapon)
            {
                var go = weapon.gameObject;
                return go;
            }
            if (obj is WeaponItemEntity weaponItem)
            {
                var go = weaponItem.gameObject;
                return go;
            }
            if (obj is BulletEntity bulletLogic)
            {
                var go = bulletLogic.gameObject;
                return go;
            }
            if (obj is BulletRendererEntity bulletRenderer)
            {
                var go = bulletRenderer.gameObject;
                return go;
            }
            if (obj is BulletItemEntity bulletItem)
            {
                var go = bulletItem.gameObject;
                return go;
            }
            if (obj is BattleArmorEntity armor)
            {
                var go = armor.gameObject;
                return go;
            }
            if (obj is BattleArmorItemEntity armorItem)
            {
                var go = armorItem.gameObject;
                return go;
            }
            if (obj is BattleEvolveItemEntity evolveItem)
            {
                var go = evolveItem.gameObject;
                return go;
            }

            Debug.LogError("Not Hanlder");
            return null;
        }

    }

}
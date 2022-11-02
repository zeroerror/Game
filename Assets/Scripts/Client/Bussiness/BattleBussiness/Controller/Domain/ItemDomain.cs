using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class ItemDomain
    {

        BattleFacades battleFacades;

        public ItemDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public GameObject SpawnItem(EntityType entityType, byte subType, int entityID, Transform parent = null)
        {
            string prefabName = GetPrefabName(entityType, subType);

            var itemAssets = battleFacades.Assets.ItemAsset;
            itemAssets.TryGetByName(prefabName, out GameObject prefab);

            var itemGo = GameObject.Instantiate(prefab);
            itemGo.SetActive(true);
            if (parent != null)
            {
                itemGo.transform.SetParent(parent.transform);
            }
            itemGo.transform.localPosition = Vector3.zero;

            CreateEntity(entityType, entityID, itemGo);

            Debug.Log($"生成物件：{prefabName}");
            return itemGo;
        }

        public void CreateEntity(EntityType entityType, int entityID, GameObject entityGo)
        {
            if (entityType == EntityType.WeaponItem)
            {
                var doamin = battleFacades.Domain.WeaponItemDomain;
                doamin.SpawnWeaponItem(entityGo, entityID);
                return;
            }

            if (entityType == EntityType.BulletItem)
            {
                var bulletItem = entityGo.GetComponent<BulletItemEntity>();
                bulletItem.Ctor();
                bulletItem.IDComponent.SetEntityId(entityID);

                var repo = battleFacades.Repo;
                var bulletItemRepo = repo.BulletItemRepo;
                bulletItemRepo.Add(bulletItem);
                return;
            }

            if (entityType == EntityType.ArmorItem)
            {
                var armorItem = entityGo.GetComponent<BattleArmorItemEntity>();
                armorItem.Ctor();
                armorItem.IDComponent.SetEntityId(entityID);

                var repo = battleFacades.Repo;
                var armorItemRepo = repo.ArmorItemRepo;
                armorItemRepo.Add(armorItem);
                return;
            }

            Debug.LogError($"没有处理的情况 {entityType.ToString()}");
        }

        public bool TryPickUpItem(EntityType entityType, ushort entityID, int masterEntityID, Transform hangPoint = null)
        {
            var repo = battleFacades.Repo;
            var roleRepo = repo.RoleLogicRepo;
            var master = roleRepo.Get(masterEntityID);

            if (entityType == EntityType.WeaponItem)
            {
                var weaponItemRepo = repo.WeaponItemRepo;
                if (weaponItemRepo.TryGetByEntityId(entityID, out var weaponItem))
                {
                    if (master.WeaponComponent.CanPickUpWeapon())
                    {
                        PickItemToWeapon(entityID, hangPoint, master, weaponItem);
                        return true;
                    }
                }

                return false;
            }

            if (entityType == EntityType.BulletItem)
            {
                // TODO: 背包容量判断
                var bulletItemRepo = repo.BulletItemRepo;
                if (bulletItemRepo.TryGet(entityID, out BulletItemEntity bulletItem))
                {
                    master.ItemComponent.TryCollectItem_Bullet(bulletItem);

                    var domain = battleFacades.Domain;
                    var bulletItemDomain = domain.BulletItemDomain;
                    bulletItemDomain.TearDownBulletItem(bulletItem);
                    return true;
                }
            }

            if (entityType == EntityType.ArmorItem)
            {
                var armorItemRepo = repo.ArmorItemRepo;
                if (armorItemRepo.TryGet(entityID, out var armorItem))
                {
                    var armorDomain = battleFacades.Domain.ArmorDomain;
                    var armor = armorDomain.SpawnArmor(armorItem.GetArmorPrefabName(), entityID);

                    master.WearArmro(armor);

                    var armorItemDomain = battleFacades.Domain.ArmorItemDomain;
                    armorItemDomain.TearDownWeaponItem(armorItem);
                    return true;
                }
            }

            if (entityType == EntityType.ArmorEvolveItem)
            {
                var armorEvolveItemRepo = repo.ArmorEvolveItemRepo;
                if (armorEvolveItemRepo.TryGet(entityID, out var armorEvolveItem))
                {
                    var armorDomain = battleFacades.Domain.ArmorDomain;

                    if (master.HasArmor())
                    {
                        var evolveTM = armorEvolveItem.evolveTM;
                        var armor = master.Armor;
                        armor.EvolveFrom(evolveTM);

                        var armorEvolveItemDomain = battleFacades.Domain.ArmorEvolveItemDomain;
                        armorEvolveItemDomain.TearDownArmorEvolveItem(armorEvolveItem);
                        return true;
                    }
                }
            }

            Debug.LogError("尚未处理的情况");
            return false;
        }

        public void PickItemToWeapon(ushort entityID, Transform hangPoint, BattleRoleLogicEntity master, WeaponItemEntity weaponItem)
        {
            var weaponDomain = battleFacades.Domain.WeaponDomain;
            var weapon = weaponDomain.SpawnWeapon(weaponItem.WeaponType, entityID);

            weapon.SetMaster(master.IDComponent.EntityID);
            master.WeaponComponent.PickUpWeapon(weapon, hangPoint);

            var weaponItemDomain = battleFacades.Domain.WeaponItemDomain;
            weaponItemDomain.TearDownWeaponItem(weaponItem);
        }

        public void DropWeaponToItem(WeaponEntity weapon)
        {
            var domain = battleFacades.Domain;
            var weaponItemDomain = domain.WeaponItemDomain;
            var weaponItem = weaponItemDomain.SpawnWeaponItem(weapon.WeaponType, weapon.IDComponent.EntityID);
            weaponItem.Ctor();

            var master = battleFacades.Repo.RoleLogicRepo.Get(weapon.MasterEntityID);
            weaponItem.transform.position = master.transform.position;

            var weaponDomain = domain.WeaponDomain;
            weaponDomain.TearDownWeapon(weapon);
        }

        string GetPrefabName(EntityType entityType, byte sortType)
        {
            string prefabName = null;
            switch (entityType)
            {
                case EntityType.WeaponItem:
                    prefabName = $"Item_Weapon_{((WeaponType)sortType).ToString()}";
                    break;
                case EntityType.BulletItem:
                    prefabName = $"Item_Bullet_{((BulletType)sortType).ToString()}";
                    break;
                case EntityType.ArmorItem:
                    prefabName = $"Item_Armor_{((ArmorType)sortType).ToString()}";
                    break;

            }

            Debug.Log($"Prefab Name:{prefabName}  entityType {entityType.ToString()}");
            return prefabName;
        }

    }

}
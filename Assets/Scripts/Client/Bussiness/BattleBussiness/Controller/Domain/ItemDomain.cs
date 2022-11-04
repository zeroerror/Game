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
            Debug.Assert(prefab != null, $"prefabName {prefabName} Asset Dont Exist");

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
                bulletItem.SetEntityID(entityID);

                var repo = battleFacades.Repo;
                var bulletItemRepo = repo.BulletItemRepo;
                bulletItemRepo.Add(bulletItem);
                return;
            }

            if (entityType == EntityType.ArmorItem)
            {
                var armorItem = entityGo.GetComponent<BattleArmorItemEntity>();
                armorItem.Ctor();
                armorItem.SetEntityID(entityID);

                var repo = battleFacades.Repo;
                var armorItemRepo = repo.ArmorItemRepo;
                armorItemRepo.Add(armorItem);
                return;
            }

            if (entityType == EntityType.EvolveItem)
            {
                var evolveItem = entityGo.GetComponent<BattleEvolveItemEntity>();
                evolveItem.Ctor();
                evolveItem.SetEntityID(entityID);

                var repo = battleFacades.Repo;
                var evolveItemRepo = repo.EvolveItemRepo;
                evolveItemRepo.Add(evolveItem);
                return;
            }

            Debug.LogError($"没有处理的情况 {entityType.ToString()}");
        }

        public bool TryPickUpItem(EntityType entityType, int weaponID, int masterEntityID, Transform hangPoint = null)
        {
            var repo = battleFacades.Repo;
            var roleRepo = repo.RoleLogicRepo;
            var master = roleRepo.Get(masterEntityID);

            if (entityType == EntityType.WeaponItem)
            {

                if (!master.WeaponComponent.CanPickUpWeapon())
                {
                    return false;
                }

                var weaponItemRepo = repo.WeaponItemRepo;
                if (!weaponItemRepo.TryGetByEntityId(weaponID, out var weaponItem))
                {
                    return false;
                }

                PickItemToWeapon(weaponID, hangPoint, master, weaponItem);
                return true;
            }

            if (entityType == EntityType.BulletItem)
            {
                var bulletItemRepo = repo.BulletItemRepo;
                if (!bulletItemRepo.TryGet(weaponID, out BulletItemEntity bulletItem))
                {
                    return false;
                }

                if (!master.ItemComponent.TryCollectItem_Bullet(bulletItem))
                {
                    return false;
                }

                var domain = battleFacades.Domain;
                var bulletItemDomain = domain.BulletItemDomain;
                bulletItemDomain.TearDownBulletItem(bulletItem);
                return true;
            }

            if (entityType == EntityType.ArmorItem)
            {
                var armorItemRepo = repo.ArmorItemRepo;
                if (!armorItemRepo.TryGet(weaponID, out var armorItem))
                {
                    return false;
                }

                var domain = battleFacades.Domain;

                var armorDomain = domain.ArmorDomain;
                var armor = armorDomain.SpawnArmor(armorItem.GetArmorPrefabName(), weaponID);
                if (!master.TryWearArmro(armor))
                {
                    return false;
                }

                var armorItemDomain = domain.ArmorItemDomain;
                armorItemDomain.TearDownWeaponItem(armorItem);
                return true;
            }

            if (entityType == EntityType.EvolveItem)
            {
                var evolveItemRepo = repo.EvolveItemRepo;
                if (!evolveItemRepo.TryGet(weaponID, out var evolveItem))
                {
                    return false;
                }

                var evolveType = evolveItem.evolveEntityType;
                
                if (evolveType == EntityType.Armor)
                {
                    if (!master.HasArmor())
                    {
                        return false;
                    }

                    Debug.Log($"护甲进化------");
                    var evolveTM = evolveItem.evolveTM;
                    var armor = master.Armor;
                    armor.EvolveFrom(evolveTM);

                    var armorEvolveItemDomain = battleFacades.Domain.ArmorEvolveItemDomain;
                    armorEvolveItemDomain.TearDownArmorEvolveItem(evolveItem);
                    return true;
                }

                if (evolveType == EntityType.BattleRole)
                {
                    Debug.Log($"人物进化------");
                    var evolveTM = evolveItem.evolveTM;
                    master.EvolveFrom(evolveTM);

                    var armorEvolveItemDomain = battleFacades.Domain.ArmorEvolveItemDomain;
                    armorEvolveItemDomain.TearDownArmorEvolveItem(evolveItem);
                    return true;
                }
            }

            Debug.LogError($"尚未处理的情况 entityType {entityType.ToString()}");
            return false;
        }

        #region [Weapon]

        public void PickItemToWeapon(int weaponID, Transform hangPoint, BattleRoleLogicEntity master, WeaponItemEntity weaponItem)
        {
            var weaponDomain = battleFacades.Domain.WeaponDomain;
            var weapon = weaponDomain.SpawnWeapon(weaponItem.WeaponType, weaponID, master.IDComponent.EntityID);

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

            var master = battleFacades.Repo.RoleLogicRepo.Get(weapon.MasterID);
            weaponItem.transform.position = master.transform.position;

            var weaponDomain = domain.WeaponDomain;
            weaponDomain.TearDownWeapon(weapon);
        }

        #endregion

        string GetPrefabName(EntityType entityType, byte sortType)
        {
            if (entityType == EntityType.WeaponItem)
            {
                return $"Item_Weapon_{((WeaponType)sortType).ToString()}";
            }
            if (entityType == EntityType.BulletItem)
            {
                return $"Item_Bullet_{((BulletType)sortType).ToString()}";
            }
            if (entityType == EntityType.ArmorItem)
            {
                return $"Item_Armor_{((ArmorType)sortType).ToString()}";
            }
            if (entityType == EntityType.EvolveItem)
            {
                return $"Item_Evolve_{(1000 + sortType).ToString()}";
            }

            return null;
        }

    }

}
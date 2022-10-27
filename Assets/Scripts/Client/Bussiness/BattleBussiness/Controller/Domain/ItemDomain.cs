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

            var idc = CreateEntity(entityType, itemGo);
            idc.SetEntityId(entityID);

            Debug.Log($"生成物件：{prefabName}");
            return itemGo;
        }

        public IDComponent CreateEntity(EntityType entityType, GameObject entityGo)
        {
            switch (entityType)
            {
                case EntityType.Weapon:
                    var weapon = entityGo.GetComponent<WeaponEntity>();
                    var weaponRepo = battleFacades.Repo.WeaponRepo;
                    weapon.Ctor();
                    weaponRepo.Add(weapon);
                    return weapon.IDComponent;
                case EntityType.BulletPack:
                    var bulletPack = entityGo.GetComponent<BulletPackEntity>();
                    var bulletPackRepo = battleFacades.Repo.BulletPackRepo;
                    bulletPack.Ctor();
                    bulletPackRepo.Add(bulletPack);
                    return bulletPack.IDComponent;
                case EntityType.Armor:
                    var armorRepo = battleFacades.Repo.ArmorRepo;
                    var armor = entityGo.GetComponent<BattleArmorEntity>();
                    armor.Ctor();
                    armorRepo.Add(armor);
                    return armor.IDComponent;
            }

            return null;
        }

        public bool TryPickUpItem(EntityType entityType, ushort entityId, int masterEntityID, Transform hangPoint = null)
        {
            var repo = battleFacades.Repo;
            var roleRepo = repo.RoleRepo;
            var master = roleRepo.Get(masterEntityID);

            if (entityType == EntityType.Weapon)
            {
                var weaponRepo = repo.WeaponRepo;
                if (weaponRepo.TryGetByEntityId(entityId, out var weaponEntity) && !weaponEntity.HasMaster && master.WeaponComponent.TryPickUpWeapon(weaponEntity, hangPoint))
                {
                    weaponEntity.SetMaster(masterEntityID);
                    weaponRepo.TryRemove(weaponEntity);
                    return true;
                }
            }

            if (entityType == EntityType.BulletPack)
            {
                // TODO: 背包容量判断
                var bulletPackRepo = repo.BulletPackRepo;
                if (bulletPackRepo.TryGet(entityId, out BulletPackEntity bulletPack))
                {
                    master.ItemComponent.TryCollectItem_Bullet(bulletPack);
                    bulletPackRepo.TryRemove(bulletPack);
                    GameObject.Destroy(bulletPack.gameObject);
                    return true;
                }
            }

            if (entityType == EntityType.Armor)
            {
                var armorRepo = repo.ArmorRepo;
                if (armorRepo.TryGet(entityId, out var armor))
                {
                    master.WearOrSwitchArmor(armor);
                    return true;
                }
            }

            Debug.LogWarning("尚未处理的情况");
            return false;
        }

        string GetPrefabName(EntityType entityType, byte sortType)
        {
            string prefabName = null;
            switch (entityType)
            {
                case EntityType.Weapon:
                    prefabName = $"Item_Weapon_{((WeaponType)sortType).ToString()}";
                    break;
                case EntityType.BulletPack:
                    prefabName = $"Item_BulletPack_{((BulletType)sortType).ToString()}";
                    break;
                case EntityType.Armor:
                    prefabName = $"Item_Armor_{((ArmorType)sortType).ToString()}";
                    break;

            }

            Debug.Log($"Prefab Name:{prefabName}  entityType {entityType.ToString()}");
            return prefabName;
        }

    }

}
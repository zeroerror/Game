using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.WorldBussiness.Interface;

namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class ItemDoamin
    {

        WorldFacades worldFacades;

        public ItemDoamin()
        {
        }

        public void Inject(WorldFacades facades)
        {
            this.worldFacades = facades;
        }

        public GameObject SpawnItem(ItemType itemType, byte sortType)
        {
            string itemName = null;
            switch (itemType)
            {
                case ItemType.Default:
                    break;
                case ItemType.Weapon:
                    itemName = ((WeaponType)sortType).ToString() + "_Item";
                    break;
                case ItemType.Bullet:
                    itemName = ((BulletType)sortType).ToString() + "_Item";
                    break;
                case ItemType.Pill:
                    break;

            }
            var itemAssets = worldFacades.Assets.ItemAsset;
            Debug.Log($"生成物件：{itemName}");
            itemAssets.TryGetByName(itemName, out GameObject item);
            item = GameObject.Instantiate(item);
            item.SetActive(true);
            return item;
        }

        public bool TryPickUpItem(ItemType itemType, ushort entityId, AllWorldRepo repo, WorldRoleLogicEntity role, Transform hangPoint = null)
        {
            bool isPickUpSucceed = false;
            switch (itemType)
            {
                case ItemType.Default:
                    break;
                case ItemType.Weapon:
                    var weaponRepo = repo.WeaponRepo;
                    if (weaponRepo.TryGetByWeaponId(entityId, out var weaponEntity))
                    {
                        isPickUpSucceed = true;
                        role.WeaponComponent.PickUpWeapon(weaponEntity, hangPoint);
                        weaponRepo.TryRemove(weaponEntity);
                    }
                    break;
                case ItemType.Bullet:
                    var bulletItemRepo = repo.BulletItemRepo;
                    if (bulletItemRepo.TryGetByBulletId(entityId, out BulletEntity bulletEntity))
                    {
                        isPickUpSucceed = true;
                        role.ItemComponent.TryCollectItem_Bullet(bulletEntity);
                        bulletItemRepo.TryRemove(bulletEntity);
                    }
                    break;
                case ItemType.Pill:
                    break;
            }

            return isPickUpSucceed;
        }

    }

}
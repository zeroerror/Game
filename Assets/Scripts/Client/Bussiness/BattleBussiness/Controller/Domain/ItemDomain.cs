using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Interface;
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

        public GameObject SpawnItem(ItemType itemType, byte sortType)
        {
            string itemName = null;
            switch (itemType)
            {
                case ItemType.Default:
                    break;
                case ItemType.Weapon:
                    itemName =$"Item_Weapon_{((WeaponType)sortType).ToString()}";
                    break;
                case ItemType.BulletPack:
                    itemName =$"Item_BulletPack_{((BulletType)sortType).ToString()}";
                    break;
                case ItemType.Pill:
                    break;

            }
            var itemAssets = battleFacades.Assets.ItemAsset;
            Debug.Log($"生成物件：{itemName}");
            itemAssets.TryGetByName(itemName, out GameObject item);
            item = GameObject.Instantiate(item);
            item.SetActive(true);
            return item;
        }

        public bool TryPickUpItem(ItemType itemType, ushort entityId, AllBattleRepo repo, BattleRoleLogicEntity master, Transform hangPoint = null)
        {
            bool isPickUpSucceed = false;
            switch (itemType)
            {
                case ItemType.Default:
                    break;
                case ItemType.Weapon:
                    var weaponRepo = repo.WeaponRepo;
                    if (weaponRepo.TryGetByEntityId(entityId, out var weaponEntity) && !weaponEntity.HasMaster && master.WeaponComponent.TryPickUpWeapon(weaponEntity, hangPoint))
                    {
                        isPickUpSucceed = true;
                        weaponEntity.SetMaster(master.IDComponent.EntityId);

                        weaponRepo.TryRemove(weaponEntity);
                    }
                    break;
                case ItemType.BulletPack:
                    var bulletPackRepo = repo.BulletPackRepo;
                    if (bulletPackRepo.TryGet(entityId, out BulletPackEntity bulletPackEntity))
                    {
                        isPickUpSucceed = true;
                        master.ItemComponent.TryCollectItem_Bullet(bulletPackEntity);
                        Debug.Log($"摧毁:{bulletPackEntity.gameObject.name}");
                        GameObject.Destroy(bulletPackEntity.gameObject);// TODO: 因为背包容量无法全部拾取的情况不能摧毁
                        bulletPackRepo.TryRemove(bulletPackEntity);
                    }
                    break;
                case ItemType.Pill:
                    break;
            }

            return isPickUpSucceed;
        }

    }

}
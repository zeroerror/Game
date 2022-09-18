using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Infrastructure.Generic;
using Game.Protocol.World;
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

        public GameObject SpawnWeapon(WeaponType weaponType)
        {
            var weapon = SpawnItem(weaponType.ToString());
            return weapon;
        }

        public GameObject SpawnItem(string itemName)
        {
            var itemAssets = worldFacades.Assets.ItemAsset;
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
                        role.WeaponComponent.PickUpWeapon(weaponEntity,hangPoint);
                        weaponRepo.TryRemove(weaponEntity);
                    }
                    break;
                case ItemType.Bullet:
                    break;
                case ItemType.Pill:
                    break;
            }

            return isPickUpSucceed;
        }

    }

}
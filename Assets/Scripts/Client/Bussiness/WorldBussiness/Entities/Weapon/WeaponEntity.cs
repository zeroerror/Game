using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Interface;
using Game.Client.Bussiness.WorldBussiness.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WeaponEntity : MonoBehaviour, IPickable
    {
        ushort weaponId;
        public ushort EntityId => weaponId;
        public void SetEntityId(ushort id) => weaponId = id;

        [SerializeField]
        WeaponType weaponType;
        public WeaponType WeaponType => weaponType;

        [SerializeField]
        ItemType itemType;
        public ItemType ItemType => itemType;
        public void SetItemType(ItemType itemType) => this.itemType = itemType;

        byte masterWRid;
        public byte MasterId => masterWRid;
        bool hasMaster;
        public bool HasMaster => hasMaster;
        public void SetMasterId(byte masterWRid)
        {
            this.masterWRid = masterWRid;
            hasMaster = true;
        }
        public void ClearMaster() => hasMaster = false;

        int bulletCapacity = 30;
        public int BulletCapacity => bulletCapacity;
        public void SetBulletCapacity(int capacity) => this.bulletCapacity = capacity;

        public int bulletNum { get; private set; }
        public void LoadBullet(int bulletNum) => this.bulletNum = bulletNum;

        public void Ctor()
        {
            itemType = ItemType.Weapon;
        }

        public int TryFireBullet(int num)
        {
            if (bulletNum >= num)
            {
                bulletNum -= num;
                return num;
            }
            else
            {
                return bulletNum;
            }
        }

    }

}
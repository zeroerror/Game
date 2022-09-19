using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Interface;
using Game.Client.Bussiness.WorldBussiness.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WeaponEntity : MonoBehaviour, IPickable
    {
        ushort weaponId;
        public ushort WeaponId => weaponId;
        public void SetEntityId(ushort id) => weaponId = id;

        [SerializeField]
        WeaponType weaponType;
        public WeaponType WeaponType => weaponType;

        [SerializeField]
        ItemType itemType;
        public ItemType ItemType => itemType;
        public void SetItemType(ItemType itemType) => this.itemType = itemType;

        public ushort EntityId => weaponId;

        byte masterWRid;
        public byte MasterId => masterWRid;
        public void SetMasterId(byte masterWRid) => this.masterWRid = masterWRid;
        public void Ctor()
        {
            itemType = ItemType.Weapon;
        }


    }

}
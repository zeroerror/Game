using System.Collections.Generic;
using Game.Client.Bussiness.WorldBussiness.Interface;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    [SerializeField]
    public enum WeaponType
    {
        // TODO: 根据武器类型去配置表查询对应弹夹容量
        // TODO: 根据武器类型去配置表查询对应所需子弹类型
        Pistol, // 手枪
        Rifle,  // 步枪
        GrenadeLauncher //榴弹发射器
    }

    public class WeaponEntity : MonoBehaviour, IPickable
    {
        ushort weaponId;
        public ushort WeaponId => weaponId;
        public void SetEntityId(ushort id)
        {
            weaponId = id;
        }

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
            SetItemType(ItemType.Weapon);
        }


    }

}
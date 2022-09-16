using System.Collections.Generic;
using Game.Client.Bussiness.WorldBussiness.Interface;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

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
        public byte MasterWRid { get; private set; }
        public void SetMasterWRid(byte wrid) => this.MasterWRid = wrid;

        ushort weaponId;
        public ushort WeaponId => weaponId;
        public void SetWeaponId(ushort id) => this.weaponId = id;

        public WeaponType WeaponType { get; private set; }
        public void SetWeaponType(WeaponType weaponType) => this.WeaponType = weaponType;


        public void Ctor()
        {
        }


    }

}
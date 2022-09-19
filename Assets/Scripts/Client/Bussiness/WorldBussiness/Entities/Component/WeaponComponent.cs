using UnityEngine;
using Game.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WeaponComponent
    {

        readonly int WEAPON_CAPICY = 4;

        public WeaponEntity[] AllWeapon;    //所有武器
        public WeaponEntity CurrentWeapon { get; private set; }  //当前武器
        public int CurrentNum { get; private set; }    //当前武器数量

        public WeaponComponent()
        {
        }

        public void Ctor()
        {
            AllWeapon = new WeaponEntity[WEAPON_CAPICY];
        }

        // 获取当前武器所需子弹
        public bool IsHoldingWeapon()
        {
            return CurrentWeapon != null;
        }

        // 拾取武器
        public void PickUpWeapon(WeaponEntity weaponEntity, Transform hangPoint = null)
        {
            if (CurrentNum >= WEAPON_CAPICY) return;

            Debug.Log($"拾取武器:{weaponEntity.WeaponType.ToString()}");
            var colliders = weaponEntity.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }
            //武器挂点
            weaponEntity.transform.SetParent(hangPoint);
            weaponEntity.transform.localPosition = Vector3.zero;
            CurrentWeapon = weaponEntity;
            CurrentWeapon.gameObject.SetActive(true);
            AllWeapon[CurrentNum++] = weaponEntity;
        }

        // 切换武器
        public void SwitchWeapon(int index)
        {
            if (index < 0 || index >= 4) return;
            //原来武器隐藏,显示新武器
            CurrentWeapon.gameObject.SetActive(false);
            CurrentWeapon = AllWeapon[index];
            CurrentWeapon.gameObject.SetActive(true);
            Debug.Log($"切换至武器{index}:{CurrentWeapon.WeaponType.ToString()}");
        }

        //丢弃武器
        public void DropWeapon()
        {

        }

    }

}
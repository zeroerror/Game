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
        public bool IsReloading { get; private set; }
        public void SetReloading(bool flag) => this.IsReloading = flag;
        public bool IsFullReloaded => CurrentWeapon.bulletNum == CurrentWeapon.BulletCapacity;


        public WeaponComponent()
        {
        }

        public void Ctor()
        {
            AllWeapon = new WeaponEntity[WEAPON_CAPICY];
        }

        public bool TryWeaponShoot()
        {
            if (CurrentWeapon == null)
            {
                Debug.LogWarning("当前尚未持有武器！");
                //TODO: 徒手攻击
                return false;
            }
            return CurrentWeapon.TryFireBullet(1) == 1;
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
        public bool TryDropWeapon(ushort entityId, out WeaponEntity weapon)
        {
            weapon = null;
            for (int i = 0; i < AllWeapon.Length; i++)
            {
                var w = AllWeapon[i];
                if (w.EntityId == entityId)
                {
                    weapon = w;
                    weapon.ClearMaster();
                    return true;
                }
            }
            return false;
        }

        public void DropWeapon(ushort entityId)
        {
            for (int i = 0; i < AllWeapon.Length; i++)
            {
                var w = AllWeapon[i];
                if (w.EntityId == entityId)
                {
                    w.ClearMaster();
                    return;
                }
            }
            return;
        }

        public bool Exist(ushort entityId)
        {
            for (int i = 0; i < AllWeapon.Length; i++)
            {
                var w = AllWeapon[i];
                if (w.EntityId == entityId) return true;
            }

            return false;
        }

    }

}
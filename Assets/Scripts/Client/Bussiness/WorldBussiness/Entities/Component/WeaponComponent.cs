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
        public bool TryPickUpWeapon(WeaponEntity weaponEntity, Transform hangPoint = null)
        {
            if (CurrentNum >= WEAPON_CAPICY)
            {
                Debug.LogWarning($"达到武器持有上限{WEAPON_CAPICY}!");
                return false;
            }

            var colliders = weaponEntity.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }

            if (hangPoint != null) Debug.Log($"{weaponEntity.transform.name} 武器挂点:{hangPoint.name}");
            weaponEntity.transform.SetParent(hangPoint);
            weaponEntity.transform.localPosition = Vector3.zero;
            CurrentWeapon = weaponEntity;
            CurrentWeapon.gameObject.SetActive(true);

            for (int i = 0; i < AllWeapon.Length; i++)
            {
                if (AllWeapon[i] == null)
                {
                    AllWeapon[i] = weaponEntity;
                    return true;
                }
            }

            return false;
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
                if (w == null) continue;

                if (w.EntityId == entityId)
                {
                    Debug.Log($"丢弃武器{entityId}");
                    weapon = w;
                    weapon.ClearMaster();

                    AllWeapon[i] = null;
                    CurrentWeapon = null;

                    CurrentNum--;

                    for (int j = 0; j < AllWeapon.Length; j++)
                    {
                        var curWeapon = AllWeapon[j];
                        if (curWeapon != null)
                        {
                            CurrentWeapon = curWeapon;
                            Debug.Log($"当前武器:{CurrentWeapon.EntityId}");
                            break;
                        }
                    }

                    return true;
                }
            }

            Debug.LogWarning($"CurrentNum:{CurrentNum} 丢弃武器失败 entityId:{entityId}");
            return false;
        }

        public void DropWeapon(ushort entityId)
        {
            for (int i = 0; i < AllWeapon.Length; i++)
            {
                var w = AllWeapon[i];
                if (w.EntityId == entityId)
                {
                    AllWeapon[i] = null;
                    w.ClearMaster();
                    return;
                }
            }
            return;
        }

        public WeaponEntity GetWeapon(ushort entityId)
        {
            for (int i = 0; i < AllWeapon.Length; i++)
            {
                var w = AllWeapon[i];
                if (w.EntityId == entityId) return w;
            }

            return null;
        }

    }

}
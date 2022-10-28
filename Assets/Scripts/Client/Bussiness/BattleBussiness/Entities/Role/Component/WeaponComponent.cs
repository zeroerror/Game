using UnityEngine;
using Game.Generic;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public class WeaponComponent
    {

        readonly int WEAPON_CAPICY = 1;

        public WeaponEntity[] AllWeapon;    //所有武器
        public WeaponEntity CurrentWeapon { get; private set; }  //当前武器
        public int CurrentNum { get; private set; }    //当前武器数量

        public bool isReloading;
        public bool IsReloading => isReloading;
        public void SetIsReloading(bool value) => isReloading = value;

        public bool IsFullReloaded => CurrentWeapon.bulletNum == CurrentWeapon.BulletCapacity;

        public void Reset()
        {
            AllWeapon = new WeaponEntity[WEAPON_CAPICY];
        }

        public void BeginReloading()
        {
            isReloading = true;
            CurrentWeapon.ResetCurrentReloadingFrame();
        }

        public void FinishReloading(int reloadBulletNum)
        {
            isReloading = false;
            CurrentWeapon.LoadBullet(reloadBulletNum);
            Debug.Log($"武器装弹：{reloadBulletNum}");
            return;
        }

        public bool TryWeaponShoot()
        {
            return CurrentWeapon.TryShootBullet(1) == 1;
        }


        // 拾取武器
        public bool CanPickUpWeapon()
        {
            if (CurrentNum >= WEAPON_CAPICY)
            {
                Debug.LogWarning($"达到武器持有上限{WEAPON_CAPICY}!");
                return false;
            }

            return true;
        }
        
        public void PickUpWeapon(WeaponEntity weaponEntity, Transform hangPoint = null)
        {
            var colliders = weaponEntity.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }

            if (hangPoint != null) Debug.Log($"{weaponEntity.transform.name} 武器挂点:{hangPoint.name}");
            weaponEntity.transform.SetParent(hangPoint);
            weaponEntity.transform.localPosition = Vector3.zero;
            weaponEntity.transform.localRotation = Quaternion.identity;
            CurrentWeapon = weaponEntity;
            CurrentWeapon.gameObject.SetActive(true);

            for (int i = 0; i < AllWeapon.Length; i++)
            {
                if (AllWeapon[i] == null)
                {
                    AllWeapon[i] = weaponEntity;
                    return;
                }
            }

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

                var entityID = w.IDComponent.EntityID;
                if (entityID == entityId)
                {
                    weapon = w;
                    weapon.Clear();

                    AllWeapon[i] = null;
                    CurrentNum--;

                    //是否为丢弃当前武器
                    if (CurrentWeapon == weapon)
                    {
                        isReloading = false;
                        CurrentWeapon = null;
                        Debug.Log($"丢弃当前武器 {w.WeaponType.ToString()}");
                    }
                    else
                    {
                        Debug.Log($"丢弃其他武器 {w.WeaponType.ToString()}");
                    }

                    for (int j = 0; j < AllWeapon.Length; j++)
                    {
                        var curWeapon = AllWeapon[j];
                        if (curWeapon != null)
                        {
                            CurrentWeapon = curWeapon;
                            Debug.Log($"当前武器:{entityId}");
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
                var entityID = w.IDComponent.EntityID;
                if (entityID == entityId)
                {
                    AllWeapon[i] = null;
                    w.Clear();
                    return;
                }
            }
            return;
        }

        public WeaponEntity GetWeapon(ushort entityID)
        {
            for (int i = 0; i < AllWeapon.Length; i++)
            {
                var w = AllWeapon[i];
                var id = w.IDComponent.EntityID;
                if (id == entityID)
                {
                    return w;
                }
            }

            return null;
        }

    }

}
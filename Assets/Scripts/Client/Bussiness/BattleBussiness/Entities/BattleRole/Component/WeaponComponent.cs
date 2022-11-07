using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public class WeaponComponent
    {

        readonly int WEAPON_CAPICY = 1;

        public WeaponEntity[] AllWeapons;    //所有武器

        WeaponEntity curWeapon; //当前武器
        public WeaponEntity CurWeapon => curWeapon;  //当前武器

        int curWeaponHoldNum;    //当前武器数量
        public int CurWeaponHoldNum => curWeaponHoldNum;  //当前武器数量

        public bool isReloading;
        public bool IsReloading => isReloading;

        public void Ctor()
        {
            AllWeapons = new WeaponEntity[WEAPON_CAPICY];
        }

        public void Reset()
        {
            for (int i = 0; i < AllWeapons.Length; i++)
            {
                var w = AllWeapons[i];
                if (w == null)
                {
                    continue;
                }
                AllWeapons[i] = null;

                w.Drop();
            }

            curWeaponHoldNum = 0;
            curWeapon = null;
        }

        public void BeginReloading()
        {
            isReloading = true;
            curWeapon.ResetCurrentReloadingFrame();
        }

        public void FinishReloading(int reloadBulletNum)
        {
            isReloading = false;
            curWeapon.LoadBullet(reloadBulletNum);
            Debug.Log($"武器装弹：{reloadBulletNum}");
            return;
        }

        public bool IsFullReloaded()
        {
            return curWeapon.BulletNum == curWeapon.BulletCapacity;
        }

        public bool TryWeaponShoot()
        {
            return curWeapon.TryShootBullet(1) == 1;
        }

        // 拾取武器
        public bool CanPickUpWeapon()
        {
            if (curWeaponHoldNum >= WEAPON_CAPICY)
            {
                Debug.LogWarning($"达到武器持有上限{WEAPON_CAPICY}!");
                return false;
            }

            return true;
        }

        public void PickUpWeapon(WeaponEntity weaponEntity, Transform hangPoint = null)
        {
            for (int i = 0; i < AllWeapons.Length; i++)
            {
                if (AllWeapons[i] == null)
                {
                    var colliders = weaponEntity.GetComponentsInChildren<Collider>();
                    for (int j = 0; j < colliders.Length; j++)
                    {
                        colliders[j].enabled = false;
                    }

                    if (hangPoint != null)
                    {
                        Debug.Log($"{weaponEntity.transform.name} 武器挂点:{hangPoint.name}");
                        weaponEntity.transform.SetParent(hangPoint);
                        weaponEntity.transform.localPosition = Vector3.zero;
                        weaponEntity.transform.localRotation = Quaternion.identity;
                    }

                    curWeapon = weaponEntity;
                    curWeapon.gameObject.SetActive(true);

                    AllWeapons[i] = curWeapon;
                    curWeaponHoldNum++;
                    return;
                }
            }
        }

        // 切换武器
        public void SwitchWeapon(int index)
        {
            if (index < 0 || index >= 4) return;
            //原来武器隐藏,显示新武器
            curWeapon.gameObject.SetActive(false);
            curWeapon = AllWeapons[index];
            curWeapon.gameObject.SetActive(true);
            Debug.Log($"切换至武器{index}:{curWeapon.WeaponType.ToString()}");
        }

        //丢弃武器
        public bool TryDropWeapon(ushort entityId, out WeaponEntity weapon)
        {
            weapon = null;
            for (int i = 0; i < AllWeapons.Length; i++)
            {
                var w = AllWeapons[i];
                if (w == null) continue;

                var entityID = w.IDComponent.EntityID;
                if (entityID == entityId)
                {
                    weapon = w;
                    weapon.Drop();

                    AllWeapons[i] = null;
                    curWeaponHoldNum--;

                    //是否为丢弃当前武器
                    if (curWeapon == weapon)
                    {
                        isReloading = false;
                        curWeapon = null;
                        Debug.Log($"丢弃当前武器 {w.WeaponType.ToString()}");
                    }
                    else
                    {
                        Debug.Log($"丢弃其他武器 {w.WeaponType.ToString()}");
                    }

                    return true;
                }
            }

            Debug.LogWarning($"丢弃武器失败");
            return false;
        }

        public WeaponEntity GetWeapon(ushort entityID)
        {
            for (int i = 0; i < AllWeapons.Length; i++)
            {
                var w = AllWeapons[i];
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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class WeaponRepo
    {

        List<WeaponEntity> weaponList;
        public int WeaponCount => (int)weaponList.Count;

        public WeaponRepo()
        {
            weaponList = new List<WeaponEntity>();
        }

        public bool TryGet(int weaponId, out WeaponEntity weaponEntity)
        {
            weaponEntity = weaponList.Find((entity) => entity.IDComponent.EntityID == weaponId);
            Debug.Assert(weaponEntity != null, $"武器{weaponId}不存在，weaponList：{weaponList.Count}");
            return weaponEntity != null;
        }

        public WeaponEntity Get(int weaponId)
        {
            var weaponEntity = weaponList.Find((entity) => entity.IDComponent.EntityID == weaponId);
            Debug.Assert(weaponEntity != null, $"武器{weaponId}不存在，weaponList：{weaponList.Count}");
            return weaponEntity;
        }

        public List<WeaponEntity> TryGetByMasterWRid(int masterWRid)
        {
            List<WeaponEntity> weaponEntityList = new List<WeaponEntity>();
            weaponList.ForEach((weapon) =>
            {
                if (weapon.MasterID == masterWRid) weaponEntityList.Add(weapon);
            });

            return weaponEntityList;
        }

        public bool TryRemove(WeaponEntity entity)
        {
            Debug.Log($"Repo:移除武器{entity.IDComponent.EntityID}");
            return weaponList.Remove(entity);
        }

        public WeaponEntity[] GetAll()
        {
            return weaponList.ToArray();
        }

        public void Add(WeaponEntity entity)
        {
            Debug.Log($"武器资源添加 [entityId:{entity.IDComponent.EntityID}]");
            weaponList.Add(entity);
        }

        public void Foreach(Action<WeaponEntity> action)
        {
            if (action == null) return;
            weaponList.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

    }

}
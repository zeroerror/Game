using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class WeaponRepo
    {

        List<WeaponEntity> all;
        public int WeaponCount => (int)all.Count;

        public WeaponRepo()
        {
            all = new List<WeaponEntity>();
        }

        public bool TryGet(int weaponId, out WeaponEntity weaponEntity)
        {
            weaponEntity = all.Find((entity) => entity.IDComponent.EntityID == weaponId);
            Debug.Assert(weaponEntity != null, $"武器{weaponId}不存在，weaponList：{all.Count}");
            return weaponEntity != null;
        }

        public WeaponEntity Get(int weaponId)
        {
            var weaponEntity = all.Find((entity) => entity.IDComponent.EntityID == weaponId);
            Debug.Assert(weaponEntity != null, $"武器{weaponId}不存在，weaponList：{all.Count}");
            return weaponEntity;
        }

        public List<WeaponEntity> TryGetByMasterWRid(int masterWRid)
        {
            List<WeaponEntity> weaponEntityList = new List<WeaponEntity>();
            all.ForEach((weapon) =>
            {
                if (weapon.MasterID == masterWRid) weaponEntityList.Add(weapon);
            });

            return weaponEntityList;
        }

        public bool TryRemove(WeaponEntity entity)
        {
            Debug.Log($"Repo:移除武器{entity.IDComponent.EntityID}");
            return all.Remove(entity);
        }

        public WeaponEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Add(WeaponEntity entity)
        {
            Debug.Log($"武器资源添加 [entityId:{entity.IDComponent.EntityID}]");
            all.Add(entity);
        }

        public void Foreach(Action<WeaponEntity> action)
        {
            if (action == null) return;
            all.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

        public void ForAll(Action<WeaponEntity> action)
        {
            if (action == null) return;
            var array = all.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                action.Invoke(array[i]);
            }
        }

    }

}
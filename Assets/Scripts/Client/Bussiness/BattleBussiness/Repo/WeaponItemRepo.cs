using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class WeaponItemRepo
    {

        List<WeaponItemEntity> all;

        public WeaponItemRepo()
        {
            all = new List<WeaponItemEntity>();
        }

        public void Add(WeaponItemEntity entity)
        {
            Debug.Log($"添加武器ITEM {entity.IDComponent.EntityID}");
            all.Add(entity);
        }

        public bool TryRemove(WeaponItemEntity entity)
        {
            Debug.Log($"移除武器ITEM {entity.IDComponent.EntityID}");
            return all.Remove(entity);
        }

        public bool TryGetByEntityId(int weaponId, out WeaponItemEntity weaponEntity)
        {
            weaponEntity = all.Find((entity) => entity.IDComponent.EntityID == weaponId);
            return weaponEntity != null;
        }

        public WeaponItemEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Foreach(Action<WeaponItemEntity> action)
        {
            if (action == null) return;
            all.ForEach((entity) =>
            {
                action.Invoke(entity);
            });
        }

        public void ForAll(Action<WeaponItemEntity> action)
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
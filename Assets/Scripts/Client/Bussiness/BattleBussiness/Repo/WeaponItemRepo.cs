using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class WeaponItemRepo
    {

        List<WeaponItemEntity> weaponItemList;

        public WeaponItemRepo()
        {
            weaponItemList = new List<WeaponItemEntity>();
        }

        public bool TryGetByEntityId(ushort weaponId, out WeaponItemEntity weaponEntity)
        {
            weaponEntity = weaponItemList.Find((entity) => entity.IDComponent.EntityID == weaponId);
            return weaponEntity != null;
        }

        public bool TryRemove(WeaponItemEntity entity)
        {
            return weaponItemList.Remove(entity);
        }

        public WeaponItemEntity[] GetAll()
        {
            return weaponItemList.ToArray();
        }

        public void Add(WeaponItemEntity entity)
        {
            weaponItemList.Add(entity);
        }

        public void Foreach(Action<WeaponItemEntity> action)
        {
            if (action == null) return;
            weaponItemList.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

    }

}
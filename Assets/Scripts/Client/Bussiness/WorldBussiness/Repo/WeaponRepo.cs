using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class WeaponRepo
    {

        List<WeaponEntity> weaponList;
        public ushort WeaponCount => (ushort)weaponList.Count;

        public WeaponRepo()
        {
            weaponList = new List<WeaponEntity>();
        }

        public WeaponEntity GetByBulletId(ushort weaponId)
        {
            return weaponList.Find((entity) => entity.WeaponId == weaponId);
        }

        public WeaponEntity GetByMasterWRid(ushort masterWRid)
        {
            return weaponList.Find((entity) => entity.MasterWRid == masterWRid);
        }

        public WeaponEntity[] GetAll()
        {
            return weaponList.ToArray();
        }

        public void Add(WeaponEntity entity)
        {
            weaponList.Add(entity);
        }

        public bool TryRemove(WeaponEntity entity)
        {
            return weaponList.Remove(entity);
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
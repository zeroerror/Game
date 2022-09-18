using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class WeaponRepo
    {

        List<WeaponEntity> weaponList;
        public ushort WeaponCount => (ushort)weaponList.Count;
        public ushort weaponIdAutoIncreaseId;

        public WeaponRepo()
        {
            weaponList = new List<WeaponEntity>();
        }

        public bool TryGetByWeaponId(ushort weaponId, out WeaponEntity weaponEntity)
        {
            weaponEntity = weaponList.Find((entity) => entity.WeaponId == weaponId);
            return weaponEntity != null;
        }

        public bool TryRemove(WeaponEntity entity)
        {
            Debug.Log($"TryRemove:{entity.WeaponId}");
            return weaponList.Remove(entity);
        }

        public bool TryGetByMasterWRid(ushort masterWRid, out WeaponEntity weaponEntity)
        {
            weaponEntity = weaponList.Find((entity) => entity.MasterId == masterWRid);
            return weaponEntity != null;
        }

        public WeaponEntity[] GetAll()
        {
            return weaponList.ToArray();
        }

        public void Add(WeaponEntity entity)
        {
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
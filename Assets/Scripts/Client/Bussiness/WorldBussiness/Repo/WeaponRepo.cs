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

        public bool TryGetByEntityId(ushort weaponId, out WeaponEntity weaponEntity)
        {
            weaponEntity = weaponList.Find((entity) => entity.EntityId == weaponId);
            return weaponEntity != null;
        }

        public List<WeaponEntity> TryGetByMasterWRid(ushort masterWRid)
        {
            List<WeaponEntity> weaponEntityList = new List<WeaponEntity>();
            weaponList.ForEach((weapon) =>
            {
                if (weapon.MasterId == masterWRid) weaponEntityList.Add(weapon);
            });

            return weaponEntityList;
        }

        public bool TryRemove(WeaponEntity entity)
        {
            Debug.Log($"TryRemove:{entity.EntityId}");
            return weaponList.Remove(entity);
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
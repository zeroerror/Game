using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class WeaponRepo
    {

        List<WeaponEntity> weaponList;
        public ushort WeaponCount => (ushort)weaponList.Count;
        public ushort WeaponIdAutoIncreaseId { get; private set; }

        public WeaponRepo()
        {
            weaponList = new List<WeaponEntity>();
        }

        public bool TryGetByEntityId(ushort weaponId, out WeaponEntity weaponEntity)
        {
            weaponEntity = weaponList.Find((entity) => entity.EntityId == weaponId);
            Debug.Assert(weaponEntity != null, $"武器{weaponId}不存在，weaponList：{weaponList.Count}");
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
            Debug.Log($"Repo:移除武器{entity.EntityId}");
            return weaponList.Remove(entity);
        }

        public WeaponEntity[] GetAll()
        {
            return weaponList.ToArray();
        }

        public void Add(WeaponEntity entity)
        {
            Debug.Log($"武器资源添加 [entityId:{entity.EntityId}]");
            weaponList.Add(entity);
            WeaponIdAutoIncreaseId++;
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
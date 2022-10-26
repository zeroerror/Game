using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BulletPackRepo
    {

        List<BulletPackEntity> bulletPackList;
        public ushort BulletPackCount => (ushort)bulletPackList.Count;
        public ushort AutoIncreaseID;

        public BulletPackRepo()
        {
            bulletPackList = new List<BulletPackEntity>();
        }

        public bool TryGet(ushort bulletId, out BulletPackEntity bulletEntity)
        {
            bulletEntity = bulletPackList.Find((entity) => entity.IDComponent.EntityID == bulletId);
            return bulletEntity != null;
        }

        public BulletPackEntity Get(ushort wRid)
        {
            return bulletPackList.Find((entity) => entity.MasterId == wRid);
        }

        public BulletPackEntity[] GetAll()
        {
            return bulletPackList.ToArray();
        }

        public void Add(BulletPackEntity entity)
        {
            bulletPackList.Add(entity);
        }

        public bool TryRemove(BulletPackEntity entity)
        {
            return bulletPackList.Remove(entity);
        }

        public void Foreach(Action<BulletPackEntity> action)
        {
            if (action == null) return;
            bulletPackList.ForEach((bulletPack) =>
            {
                action.Invoke(bulletPack);
            });
        }

    }

}
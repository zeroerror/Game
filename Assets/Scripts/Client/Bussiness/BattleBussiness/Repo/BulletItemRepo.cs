using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BulletItemRepo
    {

        List<BulletItemEntity> bulletPackList;
        public ushort BulletPackCount => (ushort)bulletPackList.Count;
        public ushort AutoIncreaseID;

        public BulletItemRepo()
        {
            bulletPackList = new List<BulletItemEntity>();
        }

        public bool TryGet(ushort bulletId, out BulletItemEntity bulletEntity)
        {
            bulletEntity = bulletPackList.Find((entity) => entity.IDComponent.EntityID == bulletId);
            return bulletEntity != null;
        }

        public BulletItemEntity Get(ushort wRid)
        {
            return bulletPackList.Find((entity) => entity.MasterId == wRid);
        }

        public BulletItemEntity[] GetAll()
        {
            return bulletPackList.ToArray();
        }

        public void Add(BulletItemEntity entity)
        {
            bulletPackList.Add(entity);
        }

        public bool TryRemove(BulletItemEntity entity)
        {
            return bulletPackList.Remove(entity);
        }

        public void Foreach(Action<BulletItemEntity> action)
        {
            if (action == null) return;
            bulletPackList.ForEach((bulletPack) =>
            {
                action.Invoke(bulletPack);
            });
        }

    }

}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class BulletItemRepo
    {

        List<BulletEntity> bulletList;
        public ushort BulletItemCount => (ushort)bulletList.Count;
        public ushort bulletItemAutoIncreaseId;

        public BulletItemRepo()
        {
            bulletList = new List<BulletEntity>();
        }

        public bool TryGetByBulletId(ushort bulletId, out BulletEntity bulletEntity)
        {
            bulletEntity = bulletList.Find((entity) => entity.EntityId == bulletId);
            return bulletEntity != null;
        }

        public BulletEntity GetByMasterId(ushort wRid)
        {
            return bulletList.Find((entity) => entity.MasterId == wRid);
        }

        public BulletEntity[] GetAll()
        {
            return bulletList.ToArray();
        }

        public void Add(BulletEntity entity)
        {
            bulletList.Add(entity);
        }

        public bool TryRemove(BulletEntity entity)
        {
            return bulletList.Remove(entity);
        }

        public void Foreach(Action<BulletEntity> action)
        {
            if (action == null) return;
            bulletList.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

    }

}
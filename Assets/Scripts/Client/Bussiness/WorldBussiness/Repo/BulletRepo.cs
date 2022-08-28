using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class BulletEntityRepo
    {

        List<BulletEntity> bulletList;
        public ushort BulletCount => (ushort)bulletList.Count;

        public BulletEntityRepo()
        {
            bulletList = new List<BulletEntity>();
        }

        public BulletEntity GetByBulletId(ushort bulletId)
        {
            return bulletList.Find((entity) => entity.BulletId == bulletId);
        }

        public BulletEntity GetByWRid(ushort wRid)
        {
            return bulletList.Find((entity) => entity.WRid == wRid);
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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BulletItemRepo
    {

        List<BulletItemEntity> bulletPackList;
        public int BulletPackCount => (int)bulletPackList.Count;

        public BulletItemRepo()
        {
            bulletPackList = new List<BulletItemEntity>();
        }

        public bool TryGet(int bulletId, out BulletItemEntity bulletEntity)
        {
            bulletEntity = bulletPackList.Find((entity) => entity.IDComponent.EntityID == bulletId);
            return bulletEntity != null;
        }

        public BulletItemEntity Get(int masterID)
        {
            return bulletPackList.Find((entity) => entity.MasterID == masterID);
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
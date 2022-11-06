using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BulletItemRepo
    {

        List<BulletItemEntity> all;

        public BulletItemRepo()
        {
            all = new List<BulletItemEntity>();
        }

        public bool TryGet(int bulletId, out BulletItemEntity bulletEntity)
        {
            bulletEntity = all.Find((entity) => entity.IDComponent.EntityID == bulletId);
            return bulletEntity != null;
        }

        public BulletItemEntity Get(int masterID)
        {
            return all.Find((entity) => entity.MasterID == masterID);
        }

        public BulletItemEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Add(BulletItemEntity entity)
        {
            Debug.Log($"添加子弹ITEM {entity.IDComponent.EntityID}");
            all.Add(entity);
        }

        public bool TryRemove(BulletItemEntity entity)
        {
            Debug.Log($"移除子弹ITEM {entity.IDComponent.EntityID}");
            return all.Remove(entity);
        }

        public void Foreach(Action<BulletItemEntity> action)
        {
            if (action == null) return;
            all.ForEach((bulletPack) =>
            {
                action.Invoke(bulletPack);
            });
        }
        
        public void ForAll(Action<BulletItemEntity> action)
        {
            if (action == null) return;
            var array = all.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                action.Invoke(array[i]);
            }
        }

    }

}
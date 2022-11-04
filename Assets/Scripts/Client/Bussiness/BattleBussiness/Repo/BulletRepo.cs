using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BulletRepo
    {

        List<BulletEntity> bulletList;

        public BulletRepo()
        {
            bulletList = new List<BulletEntity>();
        }

        public BulletEntity Get(int bulletId)
        {
            return bulletList.Find((entity) => entity.IDComponent.EntityID == bulletId);
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
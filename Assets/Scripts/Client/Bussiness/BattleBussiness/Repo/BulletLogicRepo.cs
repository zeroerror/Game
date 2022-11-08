using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BulletLogicRepo
    {

        List<BulletEntity> all;

        public BulletLogicRepo()
        {
            all = new List<BulletEntity>();
        }

        public BulletEntity Get(int bulletId)
        {
            return all.Find((entity) => entity.IDComponent.EntityID == bulletId);
        }

        public BulletEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Add(BulletEntity entity)
        {
            all.Add(entity);
        }

        public bool TryRemove(BulletEntity entity)
        {
            return all.Remove(entity);
        }

        public void Foreach(Action<BulletEntity> action)
        {
            if (action == null) return;
            all.ForEach((entity) =>
            {
                action.Invoke(entity);
            });
        }
       
        public void ForAll(Action<BulletEntity> action)
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
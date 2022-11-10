using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BulletLogicRepo
    {

        List<BulletLogicEntity> all;

        public BulletLogicRepo()
        {
            all = new List<BulletLogicEntity>();
        }

        public BulletLogicEntity Get(int bulletId)
        {
            return all.Find((entity) => entity.IDComponent.EntityID == bulletId);
        }

        public BulletLogicEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Add(BulletLogicEntity entity)
        {
            all.Add(entity);
        }

        public bool TryRemove(BulletLogicEntity entity)
        {
            return all.Remove(entity);
        }

        public void Remove(BulletLogicEntity entity)
        {
            all.Remove(entity);
        }

        public void Foreach(Action<BulletLogicEntity> action)
        {
            if (action == null) return;
            all.ForEach((entity) =>
            {
                action.Invoke(entity);
            });
        }

        public void ForAll(Action<BulletLogicEntity> action)
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
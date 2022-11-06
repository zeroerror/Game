using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BulletRendererRepo
    {

        List<BulletRendererEntity> all;

        public BulletRendererRepo()
        {
            all = new List<BulletRendererEntity>();
        }

        public BulletRendererEntity Get(int entityID)
        {
            return all.Find((entity) => entity.EntityID == entityID);
        }

        public BulletRendererEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Add(BulletRendererEntity entity)
        {
            all.Add(entity);
        }

        public bool TryRemove(BulletRendererEntity entity)
        {
            return all.Remove(entity);
        }

        public void Foreach(Action<BulletRendererEntity> action)
        {
            if (action == null) return;
            all.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }
        
        public void ForAll(Action<BulletRendererEntity> action)
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
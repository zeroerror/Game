using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class BulletEntityRepo
    {

        List<BulletEntity> list;
        public ushort Size=>(ushort)list.Count;

        public BulletEntityRepo()
        {
            list = new List<BulletEntity>();
        }

        public BulletEntity Get(byte wRid)
        {
            return list.Find((entity) => entity.WRid == wRid);
        }

        public BulletEntity[] GetAll()
        {
            return list.ToArray();
        }

        public void Add(BulletEntity entity)
        {
            list.Add(entity);
        }


        public void Remove(BulletEntity entity)
        {
            list.Remove(entity);
        }

        public void Foreach(Action<BulletEntity> action)
        {
            if (action == null) return;

            list.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

    }

}
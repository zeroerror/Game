using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BulletRepo
    {

        List<BulletEntity> bulletList;

        int autoEntityID;
        public int AutoEntityID => autoEntityID;

        public BulletRepo()
        {
            bulletList = new List<BulletEntity>();
        }

        public BulletEntity GetByBulletId(int bulletId)
        {
            return bulletList.Find((entity) => entity.IDComponent.EntityId == bulletId);
        }

        public BulletEntity GetByWRid(ushort wRid)
        {
            return bulletList.Find((entity) => entity.MasterEntityId == wRid);
        }

        public BulletEntity[] GetAll()
        {
            return bulletList.ToArray();
        }

        public void Add(BulletEntity entity)
        {
            bulletList.Add(entity);
            autoEntityID++;
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
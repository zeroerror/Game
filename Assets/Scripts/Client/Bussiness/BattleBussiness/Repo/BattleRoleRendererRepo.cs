using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleRoleRendererRepo
    {

        List<BattleRoleRendererEntity> all;

        public BattleRoleRendererRepo()
        {
            all = new List<BattleRoleRendererEntity>();
        }

        public BattleRoleRendererEntity Get(int entityID)
        {
            return all.Find((entity) => entity.EntityID == entityID);
        }

        public BattleRoleRendererEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Add(BattleRoleRendererEntity entity)
        {
            all.Add(entity);
        }

        public bool TryRemove(BattleRoleRendererEntity entity)
        {
            return all.Remove(entity);
        }

        public void Foreach(Action<BattleRoleRendererEntity> action)
        {
            if (action == null) return;
            all.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

    }

}
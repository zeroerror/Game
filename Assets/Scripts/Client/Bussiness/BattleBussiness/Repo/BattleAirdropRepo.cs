using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleAirdropRepo
    {

        List<BattleAirdropEntity> all;

        public BattleAirdropRepo()
        {
            all = new List<BattleAirdropEntity>();
        }

        public void Add(BattleAirdropEntity entity)
        {
            Debug.Log($"添加空投 [entityId:{entity.EntityID}]");
            all.Add(entity);
        }

        public bool TryRemove(BattleAirdropEntity entity)
        {
            Debug.Log($"移除空投 {entity.EntityID}");
            return all.Remove(entity);
        }

        public bool TryGet(int entityId, out BattleAirdropEntity entity)
        {
            entity = all.Find((entity) => entity.EntityID == entityId);
            Debug.Assert(entity != null, $"空投 {entityId} 不存在");
            return entity != null;
        }

        public BattleAirdropEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Foreach(Action<BattleAirdropEntity> action)
        {
            if (action == null) return;
            all.ForEach((entity) =>
            {
                action.Invoke(entity);
            });
        }

        public void ForAll(Action<BattleAirdropEntity> action)
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
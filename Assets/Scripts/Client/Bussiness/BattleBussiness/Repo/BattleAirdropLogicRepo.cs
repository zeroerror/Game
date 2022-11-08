using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleAirdropLogicRepo
    {

        List<BattleAirdropEntity> all;

        public BattleAirdropLogicRepo()
        {
            all = new List<BattleAirdropEntity>();
        }

        public void Add(BattleAirdropEntity entity)
        {
            Debug.Log($"添加空投Logic [entityId:{entity.IDComponent.EntityID}]");
            all.Add(entity);
        }

        public void Remove(BattleAirdropEntity entity)
        {
            Debug.Log($"移除空投Logic {entity.IDComponent.EntityID}");
            all.Remove(entity);
        }

        public bool TryRemove(BattleAirdropEntity entity)
        {
            Debug.Log($"移除空投Logic {entity.IDComponent.EntityID}");
            return all.Remove(entity);
        }

        public BattleAirdropEntity Get(int entityId)
        {
            var entity = all.Find((entity) => entity.IDComponent.EntityID == entityId);
            return entity;
        }

        public bool TryGet(int entityId, out BattleAirdropEntity entity)
        {
            entity = all.Find((entity) => entity.IDComponent.EntityID == entityId);
            Debug.Assert(entity != null, $"空投Logic {entityId} 不存在");
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
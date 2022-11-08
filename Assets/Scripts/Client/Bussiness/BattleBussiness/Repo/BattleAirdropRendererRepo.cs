using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleAirdropRendererRepo
    {

        List<BattleAirdropRendererEntity> all;

        public BattleAirdropRendererRepo()
        {
            all = new List<BattleAirdropRendererEntity>();
        }

        public void Add(BattleAirdropRendererEntity entity)
        {
            Debug.Log($"添加空投Renderer [entityId:{entity.EntityID}]");
            all.Add(entity);
        }

        public void Remove(BattleAirdropRendererEntity entity)
        {
            Debug.Log($"移除空投Renderer {entity.EntityID}");
            all.Remove(entity);
        }

        public void RemoveByID(int entityID)
        {
            var entity = all.Find((entity) => entity.EntityID == entityID);
            all.Remove(entity);
        }

        public bool TryRemove(BattleAirdropRendererEntity entity)
        {
            Debug.Log($"移除空投Renderer {entity.EntityID}");
            return all.Remove(entity);
        }

        public BattleAirdropRendererEntity Get(int entityId)
        {
            var entity = all.Find((entity) => entity.EntityID == entityId);
            return entity;
        }

        public bool TryGet(int entityId, out BattleAirdropRendererEntity entity)
        {
            entity = all.Find((entity) => entity.EntityID == entityId);
            Debug.Assert(entity != null, $"空投Renderer {entityId} 不存在");
            return entity != null;
        }

        public BattleAirdropRendererEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Foreach(Action<BattleAirdropRendererEntity> action)
        {
            if (action == null) return;
            all.ForEach((entity) =>
            {
                action.Invoke(entity);
            });
        }

        public void ForAll(Action<BattleAirdropRendererEntity> action)
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
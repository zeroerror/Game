using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleEvolveItemRepo
    {

        List<BattleEvolveItemEntity> all;

        public BattleEvolveItemRepo()
        {
            all = new List<BattleEvolveItemEntity>();
        }

        public bool TryGet(ushort entityId, out BattleEvolveItemEntity entity)
        {
            entity = all.Find((entity) => entity.IDComponent.EntityID == entityId);
            Debug.Assert(entity != null, $"进化ITEM {entityId} 不存在");
            return entity != null;
        }

        public bool TryRemove(BattleEvolveItemEntity entity)
        {
            Debug.Log($"移除进化ITEM {entity.IDComponent.EntityID}");
            return all.Remove(entity);
        }

        public BattleEvolveItemEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Add(BattleEvolveItemEntity entity)
        {
            Debug.Log($"添加进化ITEM [entityId:{entity.IDComponent.EntityID}]");
            all.Add(entity);
        }

        public void Foreach(Action<BattleEvolveItemEntity> action)
        {
            if (action == null) return;
            all.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

    }

}
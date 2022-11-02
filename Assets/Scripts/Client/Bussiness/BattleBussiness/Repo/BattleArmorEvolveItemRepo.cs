using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleArmorEvolveItemRepo
    {

        List<BattleArmorEvolveItemEntity> all;

        public BattleArmorEvolveItemRepo()
        {
            all = new List<BattleArmorEvolveItemEntity>();
        }

        public bool TryGet(ushort entityId, out BattleArmorEvolveItemEntity entity)
        {
            entity = all.Find((entity) => entity.IDComponent.EntityID == entityId);
            Debug.Assert(entity != null, $"进化护甲ITEM {entityId} 不存在");
            return entity != null;
        }

        public bool TryRemove(BattleArmorEvolveItemEntity entity)
        {
            Debug.Log($"移除进化护甲ITEM {entity.IDComponent.EntityID}");
            return all.Remove(entity);
        }

        public BattleArmorEvolveItemEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Add(BattleArmorEvolveItemEntity entity)
        {
            Debug.Log($"添加进化护甲ITEM [entityId:{entity.IDComponent.EntityID}]");
            all.Add(entity);
        }

        public void Foreach(Action<BattleArmorEvolveItemEntity> action)
        {
            if (action == null) return;
            all.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

    }

}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleArmorItemRepo
    {

        List<BattleArmorItemEntity> all;

        public BattleArmorItemRepo()
        {
            all = new List<BattleArmorItemEntity>();
        }


        public void Add(BattleArmorItemEntity entity)
        {
            Debug.Log($"添加护甲ITEM [entityId:{entity.IDComponent.EntityID}]");
            all.Add(entity);
        }
        
        public bool TryRemove(BattleArmorItemEntity entity)
        {
            Debug.Log($"移除护甲ITEM {entity.IDComponent.EntityID}");
            return all.Remove(entity);
        }

        public bool TryGet(ushort entityId, out BattleArmorItemEntity entity)
        {
            entity = all.Find((entity) => entity.IDComponent.EntityID == entityId);
            Debug.Assert(entity != null, $"护甲ITEM {entityId} 不存在");
            return entity != null;
        }

        public BattleArmorItemEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Foreach(Action<BattleArmorItemEntity> action)
        {
            if (action == null) return;
            all.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

    }

}
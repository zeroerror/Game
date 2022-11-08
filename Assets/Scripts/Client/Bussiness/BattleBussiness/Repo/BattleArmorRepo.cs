using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleArmorRepo
    {

        List<BattleArmorEntity> all;

        public BattleArmorRepo()
        {
            all = new List<BattleArmorEntity>();
        }

        public BattleArmorEntity Get(int entityID)
        {
            BattleArmorEntity entity = all.Find((entity) => entity.IDComponent.EntityID == entityID);
            return entity;
        }

        public bool TryGet(int entityID, out BattleArmorEntity entity)
        {
            entity = all.Find((entity) => entity.IDComponent.EntityID == entityID);
            Debug.Assert(entity != null, $"护甲 {entityID} 不存在");
            return entity != null;
        }

        public bool TryRemove(BattleArmorEntity entity)
        {
            Debug.Log($"移除护甲 {entity.IDComponent.EntityID}");
            return all.Remove(entity);
        }

        public BattleArmorEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Add(BattleArmorEntity entity)
        {
            Debug.Log($"添加护甲 [entityID:{entity.IDComponent.EntityID}]");
            all.Add(entity);
        }

        public void Foreach(Action<BattleArmorEntity> action)
        {
            if (action == null) return;
            all.ForEach((entity) =>
            {
                action.Invoke(entity);
            });
        }

        public void ForAll(Action<BattleArmorEntity> action)
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
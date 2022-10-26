using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleArmorRepo
    {

        List<BattleArmorEntity> armorList;
        ushort autoIncreaseId;
        public ushort AutoIncreaseID => autoIncreaseId;

        public BattleArmorRepo()
        {
            armorList = new List<BattleArmorEntity>();
        }

        public bool TryGet(ushort entityId, out BattleArmorEntity entity)
        {
            entity = armorList.Find((entity) => entity.IDComponent.EntityID == entityId);
            Debug.Assert(entity != null, $"护甲 {entityId} 不存在");
            return entity != null;
        }

        public bool TryRemove(BattleArmorEntity entity)
        {
            Debug.Log($"移除护甲 {entity.IDComponent.EntityID}");
            return armorList.Remove(entity);
        }

        public BattleArmorEntity[] GetAll()
        {
            return armorList.ToArray();
        }

        public void Add(BattleArmorEntity entity)
        {
            Debug.Log($"添加护甲 [entityId:{entity.IDComponent.EntityID}]");
            armorList.Add(entity);
            autoIncreaseId++;
        }

        public void Foreach(Action<BattleArmorEntity> action)
        {
            if (action == null) return;
            armorList.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

    }

}
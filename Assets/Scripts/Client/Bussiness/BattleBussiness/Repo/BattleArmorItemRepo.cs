using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleArmorItemRepo
    {

        List<BattleArmorItemEntity> armorItemList;
        ushort autoIncreaseId;
        public ushort AutoIncreaseID => autoIncreaseId;

        public BattleArmorItemRepo()
        {
            armorItemList = new List<BattleArmorItemEntity>();
        }

        public bool TryGet(ushort entityId, out BattleArmorItemEntity entity)
        {
            entity = armorItemList.Find((entity) => entity.IDComponent.EntityID == entityId);
            Debug.Assert(entity != null, $"护甲ITEM {entityId} 不存在");
            return entity != null;
        }

        public bool TryRemove(BattleArmorItemEntity entity)
        {
            Debug.Log($"移除护甲ITEM {entity.IDComponent.EntityID}");
            return armorItemList.Remove(entity);
        }

        public BattleArmorItemEntity[] GetAll()
        {
            return armorItemList.ToArray();
        }

        public void Add(BattleArmorItemEntity entity)
        {
            Debug.Log($"添加护甲ITEM [entityId:{entity.IDComponent.EntityID}]");
            armorItemList.Add(entity);
            autoIncreaseId++;
        }

        public void Foreach(Action<BattleArmorItemEntity> action)
        {
            if (action == null) return;
            armorItemList.ForEach((bullet) =>
            {
                action.Invoke(bullet);
            });
        }

    }

}
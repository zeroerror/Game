using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleRoleRepo
    {

        List<BattleRoleLogicEntity> list;
        public byte Size => (byte)list.Count;

        BattleRoleLogicEntity owner;
        public BattleRoleLogicEntity Owner => owner;
        public void SetOwner(BattleRoleLogicEntity battleRoleEntity) => this.owner = battleRoleEntity;

        public BattleRoleRepo()
        {
            list = new List<BattleRoleLogicEntity>();
        }

        public BattleRoleLogicEntity Get(int entityID)
        {
            return list.Find((entity) => entity.IDComponent.EntityID == entityID);
        }

        public bool TryGetByEntityId(int entityID, out BattleRoleLogicEntity entity)
        {
            entity = list.Find((entity) => entity.IDComponent.EntityID == entityID);
            return entity != null;
        }

        public BattleRoleLogicEntity[] GetAll()
        {
            return list.ToArray();
        }

        public void Add(BattleRoleLogicEntity entity)
        {
            list.Add(entity);
        }


        public void Remove(BattleRoleLogicEntity entity)
        {
            list.Remove(entity);
        }

        public void Foreach(Action<BattleRoleLogicEntity> action)
        {
            if (action == null) return;

            list.ForEach((role) =>
            {
                action.Invoke(role);
            });
        }

    }

}
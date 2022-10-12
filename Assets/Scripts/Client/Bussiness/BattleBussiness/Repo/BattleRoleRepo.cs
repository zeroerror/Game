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

        public BattleRoleLogicEntity GetByEntityId(byte wRid)
        {
            return list.Find((entity) => entity.IDComponent.EntityId == wRid);
        }

        public bool TryGetByEntityId(int wRid, out BattleRoleLogicEntity entity)
        {
            entity = list.Find((entity) => entity.IDComponent.EntityId == wRid);
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
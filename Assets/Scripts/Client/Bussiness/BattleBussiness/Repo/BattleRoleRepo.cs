using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Repo
{

    public class BattleRoleRepo
    {

        List<BattleRoleLogicEntity> all;
        public byte Size => (byte)all.Count;

        BattleRoleLogicEntity owner;
        public BattleRoleLogicEntity Owner => owner;
        public void SetOwner(BattleRoleLogicEntity battleRoleEntity) => this.owner = battleRoleEntity;

        public BattleRoleRepo()
        {
            all = new List<BattleRoleLogicEntity>();
        }

        public void Add(BattleRoleLogicEntity entity)
        {
            all.Add(entity);
        }

        public void Remove(BattleRoleLogicEntity entity)
        {
            all.Remove(entity);
        }

        public BattleRoleLogicEntity Get(int entityID)
        {
            return all.Find((entity) => entity.IDComponent.EntityID == entityID);
        }

        public bool TryGetByEntityId(int entityID, out BattleRoleLogicEntity entity)
        {
            entity = all.Find((entity) => entity.IDComponent.EntityID == entityID);
            return entity != null;
        }

        public BattleRoleLogicEntity[] GetAll()
        {
            return all.ToArray();
        }

        public void Foreach(Action<BattleRoleLogicEntity> action)
        {
            if (action == null) return;

            all.ForEach((role) =>
            {
                action.Invoke(role);
            });
        }

        public bool IsOwner(int entityID)
        {
            if (owner == null)
            {
                return false;
            }

            if (owner.IDComponent.EntityID != entityID)
            {
                return false;
            }

            return true;
        }

        public bool HasOwner()
        {
            return owner != null;
        }

    }

}
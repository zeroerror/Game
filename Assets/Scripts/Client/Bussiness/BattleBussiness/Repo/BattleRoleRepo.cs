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

        public bool TryGet(int entityID, out BattleRoleLogicEntity entity)
        {
            entity = all.Find((entity) => entity.IDComponent.EntityID == entityID);
            return entity != null;
        }

        public bool TryGetByConnID(int connID, out BattleRoleLogicEntity entity)
        {
            entity = all.Find((entity) => entity.ConnID == connID);
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

        public void ForAll(Action<BattleRoleLogicEntity> action)
        {
            if (action == null) return;
            var array = all.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                action.Invoke(array[i]);
            }
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
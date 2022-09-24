using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class WorldRoleRepo
    {

        List<WorldRoleEntity> list;
        public byte Size => (byte)list.Count;

        WorldRoleEntity owner;
        public WorldRoleEntity Owner => owner;
        public void SetOwner(WorldRoleEntity battleRoleEntity) => this.owner = battleRoleEntity;

        string account;
        public string Account => this.account;
        public void SetAccount(string account) => this.account = account;

        public int EntityIdAutoIncrease { get; private set; }
        public int Count => list.Count;

        public WorldRoleRepo()
        {
            list = new List<WorldRoleEntity>();
        }

        public WorldRoleEntity GetByEntityId(int entityId)
        {
            return list.Find((r) => r.EntityId == entityId);
        }

        public WorldRoleEntity GetByConnId(int connId)
        {
            return list.Find((r) => r.ConnId == connId);
        }

        public void Add(WorldRoleEntity entity)
        {
            EntityIdAutoIncrease++;
            list.Add(entity);
        }

        public void Remove(WorldRoleEntity entity)
        {
            list.Remove(entity);
        }

        public void RemoveByEntityId(int entityId)
        {
            var role = list.Find((r) => r.EntityId == entityId);
            list.Remove(role);
        }

        public WorldRoleEntity RemoveByConnId(int connId)
        {
            var role = list.Find((r) => r.ConnId == connId);
            list.Remove(role);
            return role;
        }

        public void Foreach(Action<WorldRoleEntity> action)
        {
            if (action == null) return;

            list.ForEach((role) =>
            {
                action.Invoke(role);
            });
        }

    }

}
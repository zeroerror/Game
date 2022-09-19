using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class WorldRoleRepo
    {

        List<WorldRoleLogicEntity> list;
        public byte Size => (byte)list.Count;

        WorldRoleLogicEntity owner;
        public WorldRoleLogicEntity Owner => owner;
        public void SetOwner(WorldRoleLogicEntity worldRoleEntity) => this.owner = worldRoleEntity;


        public WorldRoleRepo()
        {
            list = new List<WorldRoleLogicEntity>();
        }

        public WorldRoleLogicEntity GetByEntityId(byte wRid)
        {
            return list.Find((entity) => entity.WRid == wRid);
        }

        public WorldRoleLogicEntity[] GetAll()
        {
            return list.ToArray();
        }

        public void Add(WorldRoleLogicEntity entity)
        {
            list.Add(entity);
        }


        public void Remove(WorldRoleLogicEntity entity)
        {
            list.Remove(entity);
        }

        public void Foreach(Action<WorldRoleLogicEntity> action)
        {
            if (action == null) return;

            list.ForEach((role) =>
            {
                action.Invoke(role);
            });
        }

    }

}
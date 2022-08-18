using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class WorldRoleRepo
    {

        List<WorldRoleEntity> list;

        WorldRoleEntity owner;
        public WorldRoleEntity Owner => owner;
        public void SetOwner(WorldRoleEntity worldRoleEntity) => this.owner = worldRoleEntity;

        public WorldRoleRepo()
        {
            list = new List<WorldRoleEntity>();
        }

        public WorldRoleEntity Get(byte wRid)
        {
            return list.Find((entity) => entity.WRid == wRid);
        }

        public void Add(WorldRoleEntity entity)
        {
            list.Add(entity);
        }


        public void Remove(WorldRoleEntity entity)
        {
            list.Remove(entity);
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
using System;
using System.Collections.Generic;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class WorldRoleRepo
    {

        List<WorldRoleEntity> list;

        public WorldRoleRepo()
        {
            list = new List<WorldRoleEntity>();
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
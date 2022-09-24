using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Repo
{

    public class WorldRoomRepo
    {

        List<WorldRoomEntity> list;
        public byte Size => (byte)list.Count;

        public int EntityIdAutoIncrease { get; private set; }
        public int Count => list.Count;

        public WorldRoomRepo()
        {
            list = new List<WorldRoomEntity>();
        }

        public WorldRoomEntity GetByEntityId(int entityId)
        {
            return list.Find((r) => r.EntityId == entityId);
        }

        public void Add(WorldRoomEntity entity)
        {
            EntityIdAutoIncrease++;
            list.Add(entity);
        }

        public void Remove(WorldRoomEntity entity)
        {
            list.Remove(entity);
        }

        public void RemoveByEntityId(int entityId)
        {
            var role = list.Find((r) => r.EntityId == entityId);
            list.Remove(role);
        }

        public void Foreach(Action<WorldRoomEntity> action)
        {
            if (action == null) return;

            list.ForEach((role) =>
            {
                action.Invoke(role);
            });
        }

    }

}
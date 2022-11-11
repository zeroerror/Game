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
            var role = list.Find((r) => r.EntityID == entityId);
            list.Remove(role);
        }

        public WorldRoomEntity GetByEntityId(int entityId)
        {
            return list.Find((r) => r.EntityID == entityId);
        }

        public WorldRoomEntity[] GetAll()
        {
            return list.ToArray();
        }

        public void RemoveByMasterID(int masterID)
        {
            var worldRoom = list.Find((v) => v.MasterID == masterID);
            list.Remove(worldRoom);
        }

        public bool TryGetByMasterID(int masterID, out WorldRoomEntity worldRoom)
        {
            worldRoom = list.Find((v) => v.MasterID == masterID);
            return worldRoom != null;
        }

        public bool TryGetAll(int masterID, out WorldRoomEntity worldRoom)
        {
            worldRoom = list.Find((v) => v.MasterID == masterID);
            return worldRoom != null;
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
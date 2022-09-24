using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.Client2World;
using System.Collections.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WorldRoomEntity
    {
        string masterAccount;
        public string MasterAccount => masterAccount;
        public void SetMasterAccount(string account) => this.masterAccount = account;

        int entityId;
        public int EntityId => this.entityId;
        public void SetEntityId(int id) => this.entityId = id;

        string roomName;
        public string RoomName => roomName;
        public void SetRoomName(string roomName) => this.roomName = roomName;

        public List<int> MemberEntityIdList { get; private set; }

        public WorldRoomEntity()
        {
            MemberEntityIdList = new List<int>();
        }

    }

}
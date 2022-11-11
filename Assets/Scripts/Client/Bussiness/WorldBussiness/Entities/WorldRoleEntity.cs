using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.Client2World;
using System.Collections.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WorldRoleEntity
    {
        int entityId;
        public int EntityId => this.entityId;
        public void SetEntityId(int id) => this.entityId = id;

        string account;
        public string Account => account;
        public void SetAccount(string account) => this.account = account;

        int connId;
        public int ConnId => this.connId;
        public void SetConnID(int connId) => this.connId = connId;

    }

}
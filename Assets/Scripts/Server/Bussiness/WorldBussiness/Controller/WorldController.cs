using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Protocol.Client2World;
using Game.Server.Bussiness.WorldBussiness.Facades;
using Game.Server.Bussiness.EventCenter;
using System.Collections.Generic;
using Game.Client.Bussiness.WorldBussiness;
using Game.Protocol.World;
using Game.Infrastructure.Generic;

namespace Game.Server.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {

        List<int> connIdList;
        WorldFacades worldFacades;

        public WorldController()
        {
            ServerNetworkEventCenter.Regist_WorldConnection(OnWorldConnection);
            ServerNetworkEventCenter.Regist_WorldDisconnection(OnWorldDisconnection);

            connIdList = new List<int>();
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;

            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.RegistReq_WorldEnter(OnWorldEnterReq);
            rqs.RegistReq_WorldLeave(OnWorldLeaveReq);
            rqs.RegistReq_WorldRoomCreate(OnWorldRoomCreateReq);
        }

        public void Tick()
        {

        }

        // Client Connection
        void OnWorldConnection(int connId)
        {
            Debug.Log($"[世界服]: connID:{connId} 客户端连接成功-------------------------");
            connIdList.Add(connId);
        }
        void OnWorldDisconnection(int connId)
        {
            Debug.Log($"[世界服]: connID:{connId} 客户端断开连接-------------------------");
            connIdList.Remove(connId);
            var roleEntity = worldFacades.ClientWorldFacades.Repo.WorldRoleRepo.RemoveByConnId(connId);

            var rqs = worldFacades.Network.WorldReqAndRes;
            connIdList.ForEach((connId) =>
            {
                rqs.SendRes_WorldDisconnection(connId, roleEntity.EntityId, roleEntity.Account);
            });
        }

        #region [Client Request]
        void OnWorldEnterReq(int connId, WolrdEnterReqMessage msg)
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            var roleRepo = worldFacades.ClientWorldFacades.Repo.WorldRoleRepo;
            roleRepo.Foreach((r) =>
            {
                rqs.SendRes_WorldConnection(connId, r.EntityId, r.Account, true);
            });

            WorldRoleEntity roleEntity = new WorldRoleEntity();
            roleEntity.SetAccount(msg.account);
            roleEntity.SetEntityId(roleRepo.EntityIdAutoIncrease);
            roleEntity.SetConnId(connId);
            roleRepo.Add(roleEntity);

            Debug.Log($"[世界服]: connId:{connId} {roleEntity.EntityId},{roleEntity.Account} 客户端请求进入世界,通知当前世界所有人[数量:{connIdList.Count}]-------------------------");
            connIdList.ForEach((broadcastConnId) =>
            {
                rqs.SendRes_WorldConnection(broadcastConnId, roleEntity.EntityId, roleEntity.Account, false);
            });
        }

        void OnWorldLeaveReq(int connId, WolrdLeaveReqMessage msg)
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            var roleRepo = worldFacades.ClientWorldFacades.Repo.WorldRoleRepo;
            var roleEntity = roleRepo.RemoveByConnId(connId);
            connIdList.Remove(connId);

            Debug.Log($"[世界服]: connID:{connId} {roleEntity.EntityId},{roleEntity.Account} 客户端离开了这个世界------------------------");
            connIdList.ForEach((broadcastConnId) =>
            {
                rqs.SendRes_WorldDisconnection(broadcastConnId, roleEntity.EntityId, roleEntity.Account);
            });
        }

        void OnWorldRoomCreateReq(int connId, WorldRoomCreateReqMessage msg)
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            var roleRepo = worldFacades.ClientWorldFacades.Repo.WorldRoleRepo;
            var roleEntity = roleRepo.GetByConnId(connId);

            // 创建房间Entity
            //
            //
            var roomRepo = worldFacades.ClientWorldFacades.Repo.WorldRoomRepo;
            WorldRoomEntity roomEntity = new WorldRoomEntity();
            roomEntity.SetEntityId(roomRepo.EntityIdAutoIncrease);
            roomEntity.SetMasterAccount(roleEntity.Account);
            roomEntity.SetRoomName(msg.roomName);

            roomRepo.Add(roomEntity);

            Debug.Log($"[世界服]: connID:{connId} 玩家{roleEntity.Account} 创建了房间 id:{roomEntity.EntityId} 名称:{roomEntity.RoomName}------------------------");
            connIdList.ForEach((broadcastConnId) =>
            {
                rqs.SendRes_WorldRoomCreate(broadcastConnId,
                 roleEntity.Account,
                  roomEntity.EntityId,
                   roomEntity.RoomName,
                    NetworkConfig.LOCAL_BATTLESERVER_HOST[0],
                     NetworkConfig.BATTLESERVER_PORT[0]);
            });
            Debug.Log($"[世界服]: connID:{connId} 玩家{roleEntity.Account} 创建了房间 id:{roomEntity.EntityId} 名称:{roomEntity.RoomName}------------------------");
            Debug.Log($"[世界服]:创建战斗服 Host:{NetworkConfig.LOCAL_BATTLESERVER_HOST[0]} Port:{NetworkConfig.BATTLESERVER_PORT[0]} ------------------------");
            ServerNetworkEventCenter.Invoke_BattleServerNeedCreate();
        }

        #endregion

    }

}
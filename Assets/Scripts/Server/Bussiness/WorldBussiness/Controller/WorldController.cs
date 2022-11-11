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

        List<int> connIDList;

        ServerWorldFacades serverWorldFacades;

        public WorldController()
        {
            ServerNetworkEventCenter.Regist_WorldConnection(OnWorldConnection);
            ServerNetworkEventCenter.Regist_WorldDisconnection(OnWorldDisconnection);

            connIDList = new List<int>();
        }

        public void Inject(ServerWorldFacades worldFacades)
        {
            this.serverWorldFacades = worldFacades;

            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.RegistReq_WorldEnter(OnWorldEnterReq);
            rqs.RegistReq_WorldLeave(OnWorldLeaveReq);
            rqs.RegistReq_WorldRoomCreate(OnWorldRoomCreateReq);
            rqs.RegistReq_WorldGetAllRoomsBacisInfo(OnReq_GetWorldAllRoomsBasicInfo);
        }

        public void Tick() { }

        void OnWorldConnection(int connId)
        {
            Debug.Log($"[世界服]: ConnID: {connId} 客户端连接成功-------------------------");
            connIDList.Add(connId);
        }

        void OnWorldDisconnection(int connId)
        {
            Debug.Log($"[世界服]: ConnID: {connId} 客户端断开连接-------------------------");
            connIDList.Remove(connId);
            var roleEntity = serverWorldFacades.WorldFacades.Repo.WorldRoleRepo.RemoveByConnId(connId);

            var rqs = serverWorldFacades.Network.WorldReqAndRes;
            connIDList.ForEach((connId) =>
            {
                rqs.SendRes_WorldDisconnection(connId, roleEntity.EntityId, roleEntity.Account);
            });
        }

        void OnWorldEnterReq(int connID, WolrdEnterReqMessage msg)
        {
            var worldRqs = serverWorldFacades.Network.WorldReqAndRes;
            var repo = serverWorldFacades.WorldFacades.Repo;
            var roleRepo = repo.WorldRoleRepo;
            roleRepo.Foreach((otherWRole) =>
            {
                worldRqs.SendRes_WorldConnection(connID, otherWRole.EntityId, otherWRole.Account, false);
            });
            var entityID = roleRepo.EntityIdAutoIncrease;
            var account = msg.account;
            WorldRoleEntity wroleEntity = new WorldRoleEntity();
            wroleEntity.SetAccount(account);
            wroleEntity.SetEntityId(entityID);
            wroleEntity.SetConnID(connID);
            roleRepo.Add(wroleEntity);
            worldRqs.SendRes_WorldConnection(connID, wroleEntity.EntityId, wroleEntity.Account, true);

            Debug.Log($"[世界服]: ConnID: {connID} EntityId: {wroleEntity.EntityId} Account: {wroleEntity.Account} 客户端进入世界服,当前人数:{connIDList.Count}]-------------------------");
            connIDList.ForEach((broadcastConnId) =>
            {
                if (broadcastConnId == connID)
                {
                    return;
                }

                worldRqs.SendRes_WorldConnection(broadcastConnId, wroleEntity.EntityId, wroleEntity.Account, false);
            });
        }

        void OnWorldLeaveReq(int connID, WolrdLeaveReqMsg msg)
        {
            connIDList.Remove(connID);
            var rqs = serverWorldFacades.Network.WorldReqAndRes;

            var repo = serverWorldFacades.WorldFacades.Repo;
            var wRoleRepo = repo.WorldRoleRepo;
            var wRoleEntity = wRoleRepo.RemoveByConnId(connID);

            var entityID = wRoleEntity.EntityId;
            var account = wRoleEntity.Account;
            var worldRoomRepo = repo.WorldRoomRepo;
            if (worldRoomRepo.TryGetByMasterID(entityID, out var worldRoom))
            {
                worldRoomRepo.Remove(worldRoom);
                var roomID = worldRoom.EntityID;
                Debug.Log($"[世界服]: 房间解散 roomID: {roomID} ------------------------");
                connIDList.ForEach((broadcastConnID) =>
                {
                    rqs.SendRes_WorldRoomDismiss(broadcastConnID, roomID);
                });
            }

            Debug.Log($"[世界服]: ConnID:{connID} EntityID: {entityID} Account: {wRoleEntity.Account} 客户端离开世界服------------------------");
            connIDList.ForEach((broadcastConnId) =>
            {
                rqs.SendRes_WorldDisconnection(broadcastConnId, entityID, account);
            });
        }

        void OnWorldRoomCreateReq(int connID, WorldCreateRoomReqMsg msg)
        {
            var worldFacades = serverWorldFacades.WorldFacades;
            var repo = worldFacades.Repo;
            var rqs = serverWorldFacades.Network.WorldReqAndRes;
            var roleRepo = repo.WorldRoleRepo;
            var master = roleRepo.GetByConnId(connID);
            var masterID = master.EntityId;
            var masterAccount = master.Account;

            // - TODO : GATEWAY 
            var host = NetworkConfig.BATTLESERVER_HOST[0];
            var port = NetworkConfig.BATTLESERVER_PORT[0];

            var worldRoomRepo = repo.WorldRoomRepo;
            var roomID = worldRoomRepo.EntityIdAutoIncrease;
            WorldRoomEntity roomEntity = new WorldRoomEntity();
            roomEntity.SetEntityID(roomID);
            roomEntity.SetMasterID(master.EntityId);
            roomEntity.SetRoomName(msg.roomName);
            roomEntity.SetHost(host);
            roomEntity.SetPort(port);
            roomEntity.AddMember(masterID, masterAccount);
            worldRoomRepo.Add(roomEntity);

            connIDList.ForEach((broadcastConnID) =>
            {
                rqs.SendRes_WorldRoomCreate(broadcastConnID,
                 master.Account,
                  roomEntity.EntityID,
                   roomEntity.RoomName,
                   host,
                   port);
            });

            Debug.Log($"[世界服]: 创建房间 ID: {roomEntity.EntityID} 名称: {roomEntity.RoomName} 房主: {masterAccount}------------------------");
            Debug.Log($"[世界服]: 创建战斗服 Host: {host} Port: {port} ------------------------");
            ServerNetworkEventCenter.Invoke_StartBattleServer();
        }

        void OnReq_GetWorldAllRoomsBasicInfo(int connID, WorldAllRoomsBacisInfoReqMsg _)
        {
            var repo = serverWorldFacades.WorldFacades.Repo;
            var worldRoomRepo = repo.WorldRoomRepo;
            var worldRoomArray = worldRoomRepo.GetAll();

            var worldRqs = serverWorldFacades.Network.WorldReqAndRes;
            worldRqs.SendRes_WorldAllRoomsBacisInfo(connID, worldRoomArray);
        }

    }

}
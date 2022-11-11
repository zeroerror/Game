using Game.Infrastructure.Input;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;
using Game.Infrastructure.Network.Client;
using Game.Infrastructure.Network.Server;
using System;
using Game.Protocol.Client2World;
using UnityEngine;
using Game.Protocol.World;
using Game.Client.Bussiness.WorldBussiness;

namespace Game.Server.Bussiness.WorldBussiness.Facades
{

    public class WorldReqAndRes
    {

        NetworkServer _worldServer;

        public WorldReqAndRes()
        {
        }

        public void Inject(NetworkServer _server)
        {
            this._worldServer = _server;
        }


        // ====== Send ======

        public void SendRes_WorldConnection(int connId, int entityId, string account, bool isOwner)
        {
            WolrdEnterResMessage msg = new WolrdEnterResMessage
            {
                entityId = entityId,
                account = account,
                isOwner = isOwner
            };

            _worldServer.SendMsg(connId, msg);
        }

        public void SendRes_WorldDisconnection(int connId, int entityId, string account)
        {
            WolrdLeaveResMsg msg = new WolrdLeaveResMsg
            {
                entityId = entityId,
                account = account,
            };

            _worldServer.SendMsg(connId, msg);
        }

        public void SendRes_WorldRoomCreate(int connID, string masterAccount, int roomEntityId, string roomName, string host, ushort port)
        {
            WorldCreateRoomResMsg msg = new WorldCreateRoomResMsg
            {
                masterAccount = masterAccount,
                roomEntityId = roomEntityId,
                roomName = roomName,
                host = host,
                port = port,
            };

            _worldServer.SendMsg(connID, msg);
        }

        public void SendRes_WorldRoomDismiss(int connID, int roomEntityID)
        {
            Debug.Log("SendRes_WorldRoomDismiss");
            WorldRoomDismissResMsg msg = new WorldRoomDismissResMsg();
            msg.roomEntityID = roomEntityID;
            _worldServer.SendMsg(connID, msg);
        }

        public void SendRes_WorldAllRoomsBacisInfo(int connId, WorldRoomEntity[] allWorldRoom)
        {
            WorldAllRoomsBacisInfoResMsg msg = new WorldAllRoomsBacisInfoResMsg();
            var length = allWorldRoom.Length;
            int[] worldRoomIDs = new int[length];
            int[] worldRoomMemNums = new int[length];
            string[] worldRoomNames = new string[length];
            int[] masterIDs = new int[length];
            string[] hosts = new string[length];
            ushort[] ports = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                var worldRoom = allWorldRoom[i];
                worldRoomIDs[i] = worldRoom.EntityID;
                worldRoomNames[i] = worldRoom.RoomName;
                worldRoomMemNums[i] = worldRoom.GetMemberNum();
                hosts[i] = worldRoom.Host;
                ports[i] = worldRoom.Port;
                masterIDs[i] = worldRoom.MasterID;
            }

            msg.worldRoomIDArray = worldRoomIDs;
            msg.worldRoomNameArray = worldRoomNames;
            msg.worldRoomMemNums = worldRoomMemNums;
            msg.masterIDArray = masterIDs;
            msg.hosts = hosts;
            msg.ports = ports;

            _worldServer.SendMsg(connId, msg);
        }

        // ====== Regist ======
        public void RegistReq_WorldEnter(Action<int, WolrdEnterReqMessage> action)
        {
            _worldServer.AddRegister(action);
        }

        public void RegistReq_WorldLeave(Action<int, WolrdLeaveReqMsg> action)
        {
            _worldServer.AddRegister(action);
        }

        public void RegistReq_WorldRoomCreate(Action<int, WorldCreateRoomReqMsg> action)
        {
            _worldServer.AddRegister(action);
        }

        public void RegistReq_WorldGetAllRoomsBacisInfo(Action<int, WorldAllRoomsBacisInfoReqMsg> action)
        {
            _worldServer.AddRegister(action);
        }

    }

}
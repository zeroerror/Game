using System;
using UnityEngine;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Client2World;
using Game.Protocol.World;

namespace Game.Client.Bussiness.WorldBussiness.Network
{

    public class WorldReqAndRes
    {
        NetworkClient worldClient;

        public WorldReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            worldClient = client;
        }

        public void ConnWorldServer(string host, ushort port)
        {
            Debug.Log($"尝试连接世界服:{host}:{port}");
            worldClient.Connect(host, port);
        }

        // == Send ==
        public void SendReq_EnterWorld(string account)
        {
            WolrdEnterReqMessage msg = new WolrdEnterReqMessage
            {
                account = account
            };
            worldClient.SendMsg(msg);
            Debug.Log($"发送进入世界请求: account:{account}");
        }

        public void SendReq_LeaveWorld()
        {
            WolrdLeaveReqMsg msg = new WolrdLeaveReqMsg
            {
            };
            worldClient.SendMsg(msg);
            Debug.Log($"发送离开世界请求");
        }

        public void SendReq_GetAllWorldRoomsBasicInfo()
        {
            WorldAllRoomsBacisInfoReqMsg msg = new WorldAllRoomsBacisInfoReqMsg();
            worldClient.SendMsg(msg);
        }

        public void SendReq_CreateWorldRoom(string roomName)
        {
            WorldCreateRoomReqMsg msg = new WorldCreateRoomReqMsg
            {
                roomName = roomName
            };
            worldClient.SendMsg(msg);
            Debug.Log($"发送创建房间请求 roomName:{roomName}");
        }

        // == Regist ==

        public void RegistRes_WorldEnter(Action<WolrdEnterResMessage> action)
        {
            worldClient.RegistMsg(action);
        }

        public void RegistRes_WorldGetAllRoomsBacisInfo(Action<WorldAllRoomsBacisInfoResMsg> action)
        {
            worldClient.RegistMsg(action);
        }

        public void RegistRes_WorldLeave(Action<WolrdLeaveResMsg> action)
        {
            worldClient.RegistMsg(action);
        }

        public void RegistRes_WorldRoomCreate(Action<WorldCreateRoomResMsg> action)
        {
            worldClient.RegistMsg(action);
        }

        public void RegistRes_WorldRoomDismiss(Action<WorldRoomDismissResMsg> action)
        {
            worldClient.RegistMsg(action);
        }

    }

}
using System;
using UnityEngine;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Battle;
using Game.Protocol.Client2World;
using System.Threading;
using Game.Protocol.World;

namespace Game.Client.Bussiness.WorldBussiness.Network
{

    public class WorldReqAndRes
    {
        NetworkClient _worldServClient;

        public WorldReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            _worldServClient = client;
        }

        public void ConnWorldServer(string host, ushort port)
        {
            Debug.Log($"尝试连接世界服:{host}:{port}");
            _worldServClient.Connect(host, port);
        }

        // == Send ==
        public void SendReq_WorldEnterMsg(string account)
        {
            WolrdEnterReqMessage msg = new WolrdEnterReqMessage
            {
                account = account
            };
            _worldServClient.SendMsg(msg);
            Debug.Log($"发送进入世界请求: account:{account}");
        }

        public void SendReq_WorldLeaveMsg()
        {
            WolrdLeaveReqMessage msg = new WolrdLeaveReqMessage
            {
            };
            _worldServClient.SendMsg(msg);
            Debug.Log($"发送离开世界请求");
        }

        public void SendReq_CreateWorldRoomMsg(string roomName)
        {
            WorldRoomCreateReqMessage msg = new WorldRoomCreateReqMessage
            {
                roomName = roomName
            };
            _worldServClient.SendMsg(msg);
            Debug.Log($"发送创建房间请求 roomName:{roomName}");
        }

        // == Regist ==

        public void RegistRes_WorldEnter(Action<WolrdEnterResMessage> action)
        {
            _worldServClient.RegistMsg(action);
        }

        public void RegistRes_WorldLeave(Action<WolrdLeaveResMessage> action)
        {
            _worldServClient.RegistMsg(action);
        }

        public void RegistRes_WorldRoomCreate(Action<WorldRoomCreateResMessage> action)
        {
            _worldServClient.RegistMsg(action);
        }

    }

}
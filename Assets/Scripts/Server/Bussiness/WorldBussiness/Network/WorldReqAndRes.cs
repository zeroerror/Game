using Game.Infrastructure.Input;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;
using Game.Infrastructure.Network.Client;
using Game.Infrastructure.Network.Server;
using System;
using Game.Protocol.Client2World;
using UnityEngine;
using Game.Protocol.World;

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
            WolrdLeaveResMessage msg = new WolrdLeaveResMessage
            {
                entityId = entityId,
                account = account,
            };

            _worldServer.SendMsg(connId, msg);
        }

        public void SendRes_WorldRoomCreate(int connId, string masterAccount, int roomEntityId, string roomName, string host, ushort port)
        {
            WorldRoomCreateResMessage msg = new WorldRoomCreateResMessage
            {
                masterAccount = masterAccount,
                roomEntityId = roomEntityId,
                roomName = roomName,
                host = host,
                port = port,
            };

            _worldServer.SendMsg(connId, msg);
        }

        // ====== Regist ======
        public void RegistReq_WorldEnter(Action<int, WolrdEnterReqMessage> action)
        {
            _worldServer.AddRegister(action);
        }

        public void RegistReq_WorldLeave(Action<int, WolrdLeaveReqMessage> action)
        {
            _worldServer.AddRegister(action);
        }

        public void RegistReq_WorldRoomCreate(Action<int, WorldRoomCreateReqMessage> action)
        {
            _worldServer.AddRegister(action);
        }

    }

}
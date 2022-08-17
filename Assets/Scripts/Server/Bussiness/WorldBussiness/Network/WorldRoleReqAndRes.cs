using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.Client2World;
using Game.Protocol.World;


namespace Game.Server.Bussiness.WorldBussiness.Network
{

    public class WorldRoleReqAndRes
    {
        NetworkServer _server;

        int serverFrameIndex;

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        public void RegistReq_WorldRoleMove(Action<int, FrameReqOptMsg> action)
        {
            _server.AddRegister<FrameReqOptMsg>(action);
        }

        public void SendRes_WorldRoleMove(int connId)
        {
            serverFrameIndex++;
            FrameResOptMsg frameOptResMsg = new FrameResOptMsg
            {
                serverFrameIndex = serverFrameIndex
            };
            _server.SendMsg<FrameResOptMsg>(connId, frameOptResMsg);
        }

    }

}
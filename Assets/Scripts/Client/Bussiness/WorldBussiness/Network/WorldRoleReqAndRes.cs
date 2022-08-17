using System;
using UnityEngine;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Client2World;
using Game.Protocol.World;


namespace Game.Client.Bussiness.WorldBussiness.Network
{

    public class WorldRoleReqAndRes
    {
        NetworkClient _client;

        public WorldRoleReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            _client = client;
        }

        public void SendReq_WorldRoleMove(int frameIndex, sbyte rid, Vector3 dir)
        {
            int msg = rid << 24;
            msg |= (byte)dir.x << 16;
            msg |= (byte)dir.y << 8;
            msg |= (byte)dir.z;

            FrameReqOptMsg frameOptReqMsg = new FrameReqOptMsg
            {
                clientFrameIndex = 1,
                optTypeId = 1,
                msg = msg
            };
            _client.SendMsg<FrameReqOptMsg>(frameOptReqMsg);
        }

        public void RegistRes_WorldRoleMove(Action<FrameResOptMsg> action)
        {
            _client.RegistMsg<FrameResOptMsg>(action);
        }

    }

}
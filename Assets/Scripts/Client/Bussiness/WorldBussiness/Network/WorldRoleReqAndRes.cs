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

        public void SendReq_WorldRoleMove(int frameIndex, byte rid, Vector3 dir)
        {
            int msg = rid << 24;
            msg |= (byte)dir.x << 16;
            msg |= (byte)dir.y << 8;
            msg |= (byte)dir.z;

            FrameOptReqMsg frameOptReqMsg = new FrameOptReqMsg
            {
                clientFrameIndex = frameIndex,
                optTypeId = 1,
                msg = msg
            };
            _client.SendMsg<FrameOptReqMsg>(frameOptReqMsg);
        }

        public void RegistRes_WorldRoleOpt(Action<FrameOptResMsg> action)
        {
            _client.RegistMsg<FrameOptResMsg>(action);
        }

        public void SendReq_WolrdRoleSpawn(int frameIndex)
        {
            FrameWRoleSpawnReqMsg frameReqWRoleSpawnMsg = new FrameWRoleSpawnReqMsg
            {
                clientFrameIndex = frameIndex
            };
            _client.SendMsg<FrameWRoleSpawnReqMsg>(frameReqWRoleSpawnMsg);
        }

        public void RegistRes_WorldRoleSpawn(Action<FrameWRoleSpawnResMsg> action)
        {
            _client.RegistMsg<FrameWRoleSpawnResMsg>(action);
        }

        public void RegistResResend_WorldRoleSpawn(Action<FrameWRoleSpawnResResendMsg> action)
        {
            _client.RegistMsg<FrameWRoleSpawnResResendMsg>(action);
        }

        public void RegistResResend_Opt(Action<FrameOptResResendMsg> action)
        {
            _client.RegistMsg<FrameOptResResendMsg>(action);
        }

    }

}
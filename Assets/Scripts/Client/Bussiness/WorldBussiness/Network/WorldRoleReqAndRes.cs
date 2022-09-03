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

        public void RegistUpdate_WRole(Action<WRoleStateUpdateMsg> action)
        {
            _client.RegistMsg<WRoleStateUpdateMsg>(action);
        }

        public void SendReq_WRoleMove(int frameIndex, byte rid, Vector3 dir)
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
            _client.SendMsg(frameOptReqMsg);
        }

        public void SendReq_WRoleJump(int frameIndex, byte wRid)
        {
            FrameJumpReqMsg frameJumpReqMsg = new FrameJumpReqMsg
            {
                clientFrameIndex = frameIndex,
                wRid = wRid
            };

            _client.SendMsg(frameJumpReqMsg);
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

    }

}
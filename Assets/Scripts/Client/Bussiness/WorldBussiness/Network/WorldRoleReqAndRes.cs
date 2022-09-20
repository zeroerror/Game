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

        int clientFrame;
        public void SetClientFrame(int clienFrame) => this.clientFrame = clienFrame;

        public WorldRoleReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            _client = client;
        }

        // == Send ==
        public void SendReq_WRoleMove(byte rid, Vector3 dir)
        {
            dir.Normalize();
            ulong msg = (ulong)(ushort)rid << 48;     //16 wrid 16 x 16 y 16 z
            msg |= (ulong)(ushort)(dir.x * 100) << 32;
            msg |= (ulong)(ushort)(dir.y * 100) << 16;
            msg |= (ulong)(ushort)(dir.z * 100);

            FrameOptReqMsg frameOptReqMsg = new FrameOptReqMsg
            {
                optTypeId = 1,
                msg = msg
            };
            _client.SendMsg(frameOptReqMsg);
        }

        public void SendReq_WRoleRotate(WorldRoleLogicEntity roleEntity)
        {
            var eulerAngel = roleEntity.transform.rotation.eulerAngles;
            var rid = roleEntity.EntityId;
            ulong msg = (ulong)(ushort)rid << 48;     //16 wrid 16 x 16 y 16 z
            msg |= (ulong)(ushort)eulerAngel.x << 32;
            msg |= (ulong)(ushort)eulerAngel.y << 16;
            msg |= (ulong)(ushort)eulerAngel.z;

            FrameOptReqMsg frameOptReqMsg = new FrameOptReqMsg
            {
                optTypeId = 2,
                msg = msg
            };
            _client.SendMsg(frameOptReqMsg);
        }

        public void SendReq_WRoleJump(byte wRid)
        {
            FrameJumpReqMsg frameJumpReqMsg = new FrameJumpReqMsg
            {
                wRid = wRid
            };

            _client.SendMsg(frameJumpReqMsg);
        }

        public void SendReq_WolrdRoleSpawn()
        {
            FrameWRoleSpawnReqMsg frameReqWRoleSpawnMsg = new FrameWRoleSpawnReqMsg
            {
            };
            _client.SendMsg(frameReqWRoleSpawnMsg);
        }

        // == Regist ==

        public void RegistRes_WorldRoleSpawn(Action<FrameWRoleSpawnResMsg> action)
        {
            _client.RegistMsg<FrameWRoleSpawnResMsg>(action);
        }

        public void RegistUpdate_WRole(Action<WRoleStateUpdateMsg> action)
        {
            _client.RegistMsg<WRoleStateUpdateMsg>(action);
        }


    }

}
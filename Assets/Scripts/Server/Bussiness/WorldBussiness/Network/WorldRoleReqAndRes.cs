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

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        // == OPT ==
        public void RegistReq_WorldRoleMove(Action<int, FrameOptReqMsg> action)
        {
            _server.AddRegister<FrameOptReqMsg>(action);
        }

        public void SendRes_WorldRoleMove(int connId, int frameIndex, FrameOptReqMsg msg)
        {
            FrameOptResMsg frameOptResMsg = new FrameOptResMsg
            {
                serverFrameIndex = frameIndex,
                optTypeId = msg.optTypeId,
                msg = msg.msg
            };
            _server.SendMsg<FrameOptResMsg>(connId, frameOptResMsg);
        }

        public void ResendRes_WorldRoleMove(int connId, int frameIndex, FrameOptReqMsg msg)
        {
            var realMsg = msg.msg;
            FrameOptResResendMsg frameOptResResendMsg = new FrameOptResResendMsg
            {
                serverFrameIndex = frameIndex,
                optTypeId = msg.optTypeId,
                msg = realMsg
            };
            _server.SendMsg<FrameOptResResendMsg>(connId, frameOptResResendMsg);

            Vector3 dir = new Vector3((sbyte)(realMsg >> 16), (sbyte)(realMsg >> 8), (sbyte)realMsg);
            Debug.Log($" 服务端补发帧消息: {frameIndex} dir: {dir}----->移动");
        }

        // == SPAWN ==
        public void RegistReq_WolrdRoleSpawn(Action<int, FrameWRoleSpawnReqMsg> action)
        {
            _server.AddRegister<FrameWRoleSpawnReqMsg>(action);
        }

        public void SendRes_WorldRoleSpawn(int connId, int frameIndex, byte wRoleId,bool isOwner)
        {
            FrameWRoleSpawnResMsg frameResWRoleSpawnMsg = new FrameWRoleSpawnResMsg
            {
                serverFrameIndex = frameIndex,
                wRoleId = wRoleId,
                isOwner = isOwner
            };
            _server.SendMsg<FrameWRoleSpawnResMsg>(connId, frameResWRoleSpawnMsg);
            Debug.Log($"服务端回复帧消息 serverFrameIndex:{frameIndex} connId:{connId} ---->确认人物生成");
        }

        public void ResendRes_WorldRoleSpawn(int connId, int frameIndex, byte wRoleId, bool isOwner)
        {
            FrameWRoleSpawnResResendMsg frameWRoleSpawnResResendMsg = new FrameWRoleSpawnResResendMsg
            {
                serverFrameIndex = frameIndex,
                wRoleId = wRoleId,
                isOwner = isOwner
            };
            _server.SendMsg<FrameWRoleSpawnResResendMsg>(connId, frameWRoleSpawnResResendMsg);
            Debug.Log($"服务端补发帧消息: {frameIndex} wRoleId:{wRoleId}---->人物生成");
        }

    }

}
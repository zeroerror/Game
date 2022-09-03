using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.Client2World;
using Game.Protocol.World;
using Game.Client.Bussiness.WorldBussiness;

namespace Game.Server.Bussiness.WorldBussiness.Network
{

    public class WorldRoleReqAndRes
    {
        NetworkServer _server;

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        public void SendUpdate_WRoleState(int connId, int serverFrameIndex, WorldRoleEntity role, RoleState roleStatus, Vector3 pos, Quaternion rot, Vector3 velocity)
        {
            // Position
            int x = (int)(pos.x * 10000);
            int y = (int)(pos.y * 10000);
            int z = (int)(pos.z * 10000);

            // Rotation
            var eulerAngle = rot.eulerAngles;
            int rotX = (int)(eulerAngle.x * 10000);
            int rotY = (int)(eulerAngle.y * 10000);
            int rotZ = (int)(eulerAngle.z * 10000);

            // Velocity
            int velocityX = (int)(velocity.x * 10000);
            int velocityY = (int)(velocity.y * 10000);
            int velocityZ = (int)(velocity.z * 10000);

            var log = $"发送状态同步帧{serverFrameIndex} connId:{connId} wRid:{role.WRid} 角色状态:{roleStatus.ToString()} 位置 :{pos} 旋转角度：{eulerAngle}";
            Debug.Log($"<color=#ff0000>{log}</color>");

            WRoleStateUpdateMsg msg = new WRoleStateUpdateMsg
            {
                serverFrameIndex = serverFrameIndex,
                wRid = role.WRid,
                roleState = (int)roleStatus,
                x = x,
                y = y,
                z = z,
                eulerX = rotX,
                eulerY = rotY,
                eulerZ = rotZ,
                velocityX = velocityX,
                velocityY = velocityY,
                velocityZ = velocityZ,
                isOwner = connId == role.ConnId
            };
            _server.SendMsg<WRoleStateUpdateMsg>(connId, msg);
        }

        // == OPT ==
        public void RegistReq_WorldRoleMove(Action<int, FrameOptReqMsg> action)
        {
            _server.AddRegister<FrameOptReqMsg>(action);
        }

        public void RegistReq_Jump(Action<int, FrameJumpReqMsg> action)
        {
            _server.AddRegister(action);
        }

        // == SPAWN ==
        public void RegistReq_WolrdRoleSpawn(Action<int, FrameWRoleSpawnReqMsg> action)
        {
            _server.AddRegister<FrameWRoleSpawnReqMsg>(action);
        }

        public void SendRes_WorldRoleSpawn(int connId, int frameIndex, byte wRoleId, bool isOwner)
        {
            FrameWRoleSpawnResMsg frameResWRoleSpawnMsg = new FrameWRoleSpawnResMsg
            {
                serverFrame = frameIndex,
                wRoleId = wRoleId,
                isOwner = isOwner
            };
            _server.SendMsg<FrameWRoleSpawnResMsg>(connId, frameResWRoleSpawnMsg);
            Debug.Log($"服务端回复帧消息 serverFrameIndex:{frameIndex} connId:{connId} ---->确认人物生成");
        }

    }

}
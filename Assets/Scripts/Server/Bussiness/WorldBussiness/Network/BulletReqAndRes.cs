using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.World;


namespace Game.Server.Bussiness.WorldBussiness.Network
{

    public class BulletReqAndRes
    {
        NetworkServer _server;

        public BulletReqAndRes()
        {

        }

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        public void RegistReq_BulletSpawn(Action<int, FrameBulletSpawnReqMsg> action)
        {
            _server.AddRegister<FrameBulletSpawnReqMsg>(action);
        }

        public void SendRes_BulletSpawn(int connId, int frameIndex, byte bulletType, ushort bulletId, byte wRid, Vector3 dir)
        {
            FrameBulletSpawnResMsg msg = new FrameBulletSpawnResMsg
            {
                serverFrame = frameIndex,
                wRid = wRid,
                bulletType = bulletType,
                bulletId = bulletId,
                shootDirX = (short)(dir.x * 100),
                shootDirY = (short)(dir.y * 100),
                shootDirZ = (short)(dir.z * 100),
            };

            _server.SendMsg(connId, msg);
            // Debug.Log($"dir.z:{dir.z} shootDirZ :{msg.shootDirZ}");
        }

        public void SendRes_BulletHitRole(int connId, int frame, ushort bulletId, byte wRid)
        {
            FrameBulletHitRoleResMsg msg = new FrameBulletHitRoleResMsg
            {
                serverFrame = frame,
                bulletId = bulletId,
                wRid = wRid
            };

            _server.SendMsg(connId, msg);
        }

    }

}
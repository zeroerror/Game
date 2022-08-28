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

        public void SendRes_BulletSpawn(int connId, int frameIndex, ushort bulletId, byte wRid, Vector3 dir)
        {
            FrameBulletSpawnResMsg msg = new FrameBulletSpawnResMsg
            {
                serverFrameIndex = frameIndex,
                wRid = wRid,
                bulletId = bulletId,
                shootDirX = (short)(dir.x * 100),
                shootDirY = (short)(dir.y * 100),
                shootDirZ = (short)(dir.z * 100),
            };

            _server.SendMsg(connId, msg);
        }

    }

}
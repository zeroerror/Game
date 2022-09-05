using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.World;
using Game.Client.Bussiness.WorldBussiness;

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

        public void SendRes_BulletTearDown(int connId, int serverFrame, BulletType bulletType, byte wRid, ushort bulletId, Vector3 pos)
        {
            Debug.Log($"子弹销毁消息发送: serverFrame：{serverFrame} wRid：{wRid}");
            FrameBulletTearDownResMsg msg = new FrameBulletTearDownResMsg
            {
                serverFrame = serverFrame,
                bulletType = (byte)bulletType,
                wRid = wRid,
                bulletId = bulletId,
                posX = (int)(pos.x * 10000f),  // (16 16) 整数部16位 short -32768 --- +32767 小数部分16位 ushort(0 --- +65535) 0.0000到0.9999
                posY = (int)(pos.y * 10000f),
                posZ = (int)(pos.z * 10000f)
            };

            _server.SendMsg(connId, msg);
        }


        // == Regist ==
        public void RegistReq_BulletSpawn(Action<int, FrameBulletSpawnReqMsg> action)
        {
            _server.AddRegister<FrameBulletSpawnReqMsg>(action);
        }

    }

}
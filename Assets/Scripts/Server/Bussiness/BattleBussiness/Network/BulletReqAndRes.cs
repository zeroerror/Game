using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.Battle;
using Game.Client.Bussiness.BattleBussiness;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Server.Bussiness.BattleBussiness.Network
{

    public class BulletReqAndRes
    {
        NetworkServer _server;
        int serverFrame;
        public void SetServerFrame(int serveFrame) => this.serverFrame = serveFrame;

        int sendCount;
        public int SendCount => sendCount;
        public void ClearSendCount() => sendCount = 0;

        public BulletReqAndRes()
        {

        }

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        // ====== Send ======

        public void SendRes_BulletSpawn(int connId, BulletType bulletType, ushort bulletId, byte wRid, Vector3 dir)
        {
            FrameBulletSpawnResMsg msg = new FrameBulletSpawnResMsg
            {
                serverFrame = serverFrame,
                wRid = wRid,
                bulletType = (byte)bulletType,
                bulletId = bulletId,
                shootDirX = (short)(dir.x * 100),
                shootDirY = (short)(dir.y * 100),
                shootDirZ = (short)(dir.z * 100),
            };

            _server.SendMsg(connId, msg);
            // Debug.Log($"dir.z:{dir.z} shootDirZ :{msg.shootDirZ}");
            sendCount++;
        }

        public void SendRes_BulletHitRole(int connId, ushort bulletId, byte wRid)
        {
            FrameBulletHitRoleResMsg msg = new FrameBulletHitRoleResMsg
            {
                serverFrame = serverFrame,
                bulletId = bulletId,
                wRid = wRid
            };

            _server.SendMsg(connId, msg);
            sendCount++;
        }

        public void SendRes_BulletHitWall(int connId, BulletEntity bulletEntity)
        {
            var bulletPos = bulletEntity.MoveComponent.CurPos;
            Debug.Log($"SendRes_BulletHitWall: bulletPos  {bulletPos}");
            bulletPos *= 10000f;
            FrameBulletHitWallResMsg msg = new FrameBulletHitWallResMsg
            {
                serverFrame = serverFrame,
                bulletId = bulletEntity.EntityId,
                posX = (int)bulletPos.x,
                posY = (int)bulletPos.y,
                posZ = (int)bulletPos.z,
            };

            _server.SendMsg(connId, msg);
            sendCount++;
        }

        public void SendRes_BulletTearDown(int connId, BulletType bulletType, byte wRid, ushort bulletId, Vector3 pos)
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
            sendCount++;
        }


        // == Regist ==
        public void RegistReq_BulletSpawn(Action<int, FrameBulletSpawnReqMsg> action)
        {
            _server.AddRegister<FrameBulletSpawnReqMsg>(action);
        }

    }

}
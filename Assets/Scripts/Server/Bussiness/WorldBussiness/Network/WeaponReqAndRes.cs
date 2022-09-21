using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.World;
using Game.Client.Bussiness.WorldBussiness;

namespace Game.Server.Bussiness.WorldBussiness.Network
{

    public class WeaponReqAndRes
    {
        NetworkServer _server;
        int serverFrame;
        public void SetServerFrame(int serveFrame) => this.serverFrame = serveFrame;

        int sendCount;
        public int SendCount => sendCount;
        public void ClearSendCount() => sendCount = 0;

        public WeaponReqAndRes()
        {

        }

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        // ====== Send ======
        public void SendRes_WeaponShoot(int connId, byte masterId)
        {
            FrameWeaponShootResMsg msg = new FrameWeaponShootResMsg
            {
                masterId = masterId,
            };
            _server.SendMsg(connId, msg);
            sendCount++;
            Debug.Log("回复武器射击请求");
        }

        public void SendRes_WeaponReloaded(int connId, int serverFrame, byte masterId, int reloadBulletNum)
        {
            FrameWeaponReloadResMsg msg = new FrameWeaponReloadResMsg
            {
                masterId = masterId,
                reloadBulletNum = (byte)reloadBulletNum
            };
            _server.SendMsg(connId, msg);
            sendCount++;
            Debug.Log("回复武器装弹请求");
        }

        public void SendRes_WeaponDrop(int connId, int serverFrame, byte masterId, ushort entityId)
        {
            FrameWeaponDropResMsg msg = new FrameWeaponDropResMsg
            {
                entityId = entityId,
                masterId = masterId,
            };
            _server.SendMsg(connId, msg);
            sendCount++;
            Debug.Log("回复武器丢弃请求");
        }

        // ====== Regist ======
        public void RegistReq_WeaponShoot(Action<int, FrameWeaponShootReqMsg> action)
        {
            _server.AddRegister(action);
        }

        public void RegistReq_WeaponReload(Action<int, FrameWeaponReloadReqMsg> action)
        {
            _server.AddRegister(action);
        }

        public void RegistReq_WeaponDrop(Action<int, FrameWeaponDropReqMsg> action)
        {
            _server.AddRegister(action);
        }


    }
}
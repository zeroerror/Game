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

        public WeaponReqAndRes()
        {

        }

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        // ====== Send ======
        public void SendRes_WeaponReloaded(int connId, int serverFrame, ushort entityId)
        {
            FrameWeaponReloadReqMsg msg = new FrameWeaponReloadReqMsg
            {
                entityId = entityId,
            };
            _server.SendMsg(connId, msg);
        }

        public void SendRes_WeaponDrop(int connId, int serverFrame, ushort entityId)
        {
            FrameWeaponDropResMsg msg = new FrameWeaponDropResMsg
            {
                entityId = entityId,
            };
            _server.SendMsg(connId, msg);
        }

        // ====== Regist ======
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
using System;
using UnityEngine;
using Game.Protocol.World;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness.Network
{

    public class WeaponReqAndRes
    {
        NetworkClient _client;

        int clientFrame;
        public void SetClientFrame(int clientFrame) => this.clientFrame = clientFrame;

        public WeaponReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            _client = client;
        }


        // ====== Send ======
        public void SendReq_WeaponReload(byte wRid, ushort entityId)
        {
            FrameWeaponDropReqMsg frameDropWeaponReqMsg = new FrameWeaponDropReqMsg
            {
                entityId = entityId
            };
            _client.SendMsg(frameDropWeaponReqMsg);
        }

        public void SendReq_WeaponDrop(byte wRid, ushort entityId)
        {
            FrameWeaponReloadReqMsg frameWeaponReloadReqMsg = new FrameWeaponReloadReqMsg
            {
                entityId = entityId
            };
            _client.SendMsg(frameWeaponReloadReqMsg);
        }

        public void SendReq_WeaponDrop()
        {

        }

        // ====== Regist ======
        public void RegistRes_WeaponReload(Action<FrameWeaponReloadReqMsg> action)
        {
            _client.RegistMsg(action);
        }

        public void RegistRes_WeaponDrop(Action<FrameWeaponDropReqMsg> action)
        {
            _client.RegistMsg(action);
        }

    }

}
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
        public void SendReq_WeaponShoot(byte masterId, Vector3 targetPos)
        {
            targetPos *= 10000f;
            FrameWeaponShootReqMsg frameWeaponShootReqMsg = new FrameWeaponShootReqMsg
            {
                masterId = masterId,
                targetPosX = (int)targetPos.x,
                targetPosY = (int)targetPos.y,
                targetPosZ = (int)targetPos.z,
            };
            _client.SendMsg(frameWeaponShootReqMsg);
        }

        public void SendReq_WeaponReload(WorldRoleLogicEntity role)
        {
            FrameWeaponReloadReqMsg frameWeaponReloadReqMsg = new FrameWeaponReloadReqMsg
            {
                masterId = role.EntityId,
            };
            _client.SendMsg(frameWeaponReloadReqMsg);
            Debug.Log("发送武器装弹请求");
        }

        public void SendReq_WeaponDrop(WorldRoleLogicEntity role)
        {
            if (role.WeaponComponent.CurrentWeapon == null) return;

            FrameWeaponDropReqMsg frameWeaponDropReqMsg = new FrameWeaponDropReqMsg
            {
                entityId = role.WeaponComponent.CurrentWeapon.EntityId,
                masterId = role.EntityId
            };
            _client.SendMsg(frameWeaponDropReqMsg);
            Debug.Log("发送武器丢弃请求");
        }

        // ====== Regist ======
        public void RegistRes_WeaponShoot(Action<FrameWeaponShootResMsg> action)
        {
            _client.RegistMsg(action);
        }

        public void RegistRes_WeaponReload(Action<FrameWeaponReloadResMsg> action)
        {
            _client.RegistMsg(action);
        }

        public void RegistRes_WeaponDrop(Action<FrameWeaponDropResMsg> action)
        {
            _client.RegistMsg(action);
        }

    }

}
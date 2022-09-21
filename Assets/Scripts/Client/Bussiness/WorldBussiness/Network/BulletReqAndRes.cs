using System;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class BulletReqAndRes
    {
        NetworkClient _client;

        public BulletReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            _client = client;
        }

        public void SendReq_BulletSpawn(BulletType bulletType, byte wRid, Vector3 targetPos)
        {
            targetPos *= 10000f;

            FrameBulletSpawnReqMsg msg = new FrameBulletSpawnReqMsg
            {
                bulletType = (byte)bulletType,
                wRid = wRid,
                targetPosX = (int)(targetPos.x),
                targetPosY = (int)(targetPos.y),
                targetPosZ = (int)(targetPos.z)
            };
            _client.SendMsg(msg);
            Debug.Log($"发送生成子弹网络请求:wRid:{wRid} 目标点：{targetPos}");
        }

        public void RegistRes_BulletSpawn(Action<FrameBulletSpawnResMsg> action)
        {
            _client.RegistMsg(action);
        }

        public void RegistRes_BulletTearDown(Action<FrameBulletTearDownResMsg> action)
        {
            _client.RegistMsg(action);
        }

        public void RegistRes_BulletHitRole(Action<FrameBulletHitRoleResMsg> action)
        {
            _client.RegistMsg(action);
        }

        public void RegistRes_BulletHitWall(Action<FrameBulletHitWallResMsg> action)
        {
            _client.RegistMsg(action);
        }

    }

}


using System;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class BattleReqAndRes
    {
        NetworkClient battleClient;

        public BattleReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            battleClient = client;
        }

        public void ConnBattleServer(string host, ushort port)
        {
            Debug.Log($"尝试连接战斗服:{host}:{port}");
            battleClient.Connect(host, port);
        }

        // ====== Send ======
        public void SendReq_HeartBeat()
        {
            BattleHeartbeatReqMsg battleHeartbeatReqMsg = new BattleHeartbeatReqMsg
            {
            };

            battleClient.SendMsg(battleHeartbeatReqMsg);
            Debug.Log($"发送心跳消息");
        }

        // ====== Regist ======
        public void RegistRes_HeartBeat(Action<BattleHeartbeatResMsg> action)
        {
            battleClient.RegistMsg(action);
            Debug.Log($"收到心跳消息");
        }

    }

}
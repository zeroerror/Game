

using System;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Server;
using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;

namespace Game.Server.Bussiness.BattleBussiness.Network
{

    public class BattleReqAndRes
    {
        NetworkServer battleServer;
        int serverFrame;
        public void SetServerFrame(int serveFrame) => this.serverFrame = serveFrame;

        int sendCount;
        public int SendCount => sendCount;
        public void ClearSendCount() => sendCount = 0;

        public BattleReqAndRes()
        {

        }

        public void Inject(NetworkServer server)
        {
            battleServer = server;
        }

        // ====== Send ======
        public void SendRes_HeartBeat(int connId)
        {
            BattleHeartbeatResMsg msg = new BattleHeartbeatResMsg
            {
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log($"发送心跳回复消息");
        }

        // ====== Regist ======
        public void RegistReq_HeartBeat(Action<int, BattleHeartbeatReqMsg> action)
        {
            battleServer.AddRegister(action);
            Debug.Log($"收到心跳消息");
        }

    }

}
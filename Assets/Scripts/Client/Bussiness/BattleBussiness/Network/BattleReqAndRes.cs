

using System;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.BattleBussiness.Generic;
using ZeroFrame.Protocol;
using System.Collections.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class BattleReqAndRes
    {
        NetworkClient battleClient;
        List<Action> actionList;

        public BattleReqAndRes()
        {
            actionList = new List<Action>();
        }

        public void Inject(NetworkClient client)
        {
            battleClient = client;
        }

        public void TickAllRegistAction()
        {
            for (int i = 0; i < actionList.Count; i++)
            {
                var action = actionList[i];
                action.Invoke();
            }
            actionList.Clear();
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
            AddRegister(action);
            Debug.Log($"收到心跳消息");
        }

        // Private Func
        void AddRegister<T>(Action<T> action) where T : IZeroMessage<T>, new()
        {
            lock (actionList)
            {
                battleClient.RegistMsg<T>((msg) =>
                {
                    actionList.Add(() =>
                    {
                        action.Invoke(msg);
                    });
                });
            }
        }

    }

}
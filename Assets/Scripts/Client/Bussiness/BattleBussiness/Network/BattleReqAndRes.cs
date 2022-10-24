

using System;
using UnityEngine;
using Game.Infrastructure.Network.Client;
using ZeroFrame.Protocol;
using System.Collections.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class BattleReqAndRes
    {
        NetworkClient battleClient;
        List<Action> actionList;
        object lockObj;

        public BattleReqAndRes()
        {
            actionList = new List<Action>();
            lockObj = this;
        }

        public void Inject(NetworkClient client)
        {
            battleClient = client;
        }

        public void TickAllRegistAction()
        {
            lock (lockObj)
            {
                for (int i = 0; i < actionList.Count; i++)
                {
                    var action = actionList[i];
                    action.Invoke();
                }
                actionList.Clear();
            }
        }

        public void ConnBattleServer(string host, ushort port)
        {
            Debug.Log($"尝试连接战斗服:{host}:{port}");
            battleClient.Connect(host, port);
        }

        // ====== Send ======
        // ====== Regist ======

        void AddRegister<T>(Action<T> action) where T : IZeroMessage<T>, new()
        {
            lock (lockObj)
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
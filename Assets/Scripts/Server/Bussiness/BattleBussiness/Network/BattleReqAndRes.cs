

using System;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Server;
using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;
using System.Collections.Generic;
using ZeroFrame.Protocol;

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

        List<Action> actionList;

        public BattleReqAndRes()
        {
            actionList = new List<Action>();
        }

        public void Inject(NetworkServer server)
        {
            battleServer = server;
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

        // ====== Send ======
        // ====== Regist ======

        void AddRegister<T>(Action<int, T> action) where T : IZeroMessage<T>, new()
        {
            battleServer.AddRegister<T>((connId, msg) =>
            {
                actionList.Add(() =>
                {
                    action?.Invoke(connId, msg);
                });
            });
        }

    }

}
using System;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Server;
using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;
using ZeroFrame.Protocol;
using System.Collections.Generic;

namespace Game.Server.Bussiness.BattleBussiness.Network
{

    public class ItemReqAndRes
    {
        NetworkServer battleServer;
        int serverFrame;
        public void SetServerFrame(int serveFrame) => this.serverFrame = serveFrame;

        int sendCount;
        public int SendCount => sendCount;
        public void ClearSendCount() => sendCount = 0;

        List<Action> actionList;

        public ItemReqAndRes()
        {
            actionList = new List<Action>();
            actionList.Clear();
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

        #region [Send]

        public void SendRes_ItemPickUp(int connId, int frameIndex, byte wRid, ItemType itemType, ushort entityId)
        {
            FrameItemPickResMsg msg = new FrameItemPickResMsg
            {
                serverFrame = frameIndex,
                wRid = wRid,
                itemType = (byte)itemType,
                entityId = entityId
            };
            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log($"SendRes_ItemPickUp connId:{connId}");
        }

        public void SendRes_ItemSpawn(int connId, int frameIndex, byte[] itemTypeArray, byte[] subtypeArray, ushort[] entityIdArray)
        {
            FrameItemSpawnResMsg msg = new FrameItemSpawnResMsg
            {
                serverFrame = frameIndex,
                itemTypeArray = itemTypeArray,
                subtypeArray = subtypeArray,
                entityIdArray = entityIdArray
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
        }

        #endregion

        #region [Regist]

        public void RegistReq_ItemPickUp(Action<int, FrameItemPickReqMsg> action)
        {
            AddRegister(action);
        }

        // Private Func
        void AddRegister<T>(Action<int, T> action) where T : IZeroMessage<T>, new()
        {
            battleServer.AddRegister<T>((connId, msg) =>
            {
                actionList.Add(() =>
                {
                    action.Invoke(connId, msg);
                });
            });
        }

        #endregion

    }

}
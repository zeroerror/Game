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
                action?.Invoke();
            }
            actionList.Clear();
        }

        #region [Send]

        public void SendRes_ItemPickUp(int connId, int frameIndex, int roleID, EntityType itemType, int itemID)
        {
            FrameItemPickResMsg msg = new FrameItemPickResMsg
            {
                serverFrame = frameIndex,
                roleID = (byte)roleID,
                entityType = (byte)itemType,
                itemID = (ushort)itemID
            };
            battleServer.SendMsg(connId, msg);
            sendCount++;
        }

        public void SendRes_ItemSpawn(int connId, List<EntityType> entityTypeList, List<byte> subtypeList, List<int> entityIDList)
        {
            byte[] entityTypeByteArray = new byte[entityTypeList.Count];
            for (int i = 0; i < entityTypeByteArray.Length; i++)
            {
                entityTypeByteArray[i] = (byte)entityTypeList[i];
            }

            FrameItemSpawnResMsg msg = new FrameItemSpawnResMsg
            {
                serverFrame = serverFrame,
                entityTypeArray = entityTypeByteArray,
                subtypeArray = subtypeList.ToArray(),
                entityIDArray = entityIDList.ToArray()
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
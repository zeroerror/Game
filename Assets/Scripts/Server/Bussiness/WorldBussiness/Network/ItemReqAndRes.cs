using System;
using Game.Protocol.World;
using Game.Infrastructure.Network.Server;
using Game.Client.Bussiness.WorldBussiness.Generic;
using UnityEngine;

namespace Game.Server.Bussiness.WorldBussiness.Network
{

    public class ItemReqAndRes
    {
        NetworkServer _server;
        int serverFrame;
        public void SetServerFrame(int serveFrame) => this.serverFrame = serveFrame;

        int sendCount;
        public int SendCount => sendCount;
        public void ClearSendCount() => sendCount = 0;

        public ItemReqAndRes()
        {

        }

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        // ====== Send ======
        public void SendRes_ItemPickUp(int connId, int frameIndex, byte wRid, ItemType itemType, ushort entityId)
        {
            FrameItemPickResMsg msg = new FrameItemPickResMsg
            {
                serverFrame = frameIndex,
                wRid = wRid,
                itemType = (byte)itemType,
                entityId = entityId
            };
            _server.SendMsg(connId, msg);
            sendCount++;
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

            _server.SendMsg(connId, msg);
            sendCount++;
        }

        // ====== Regist ======
        public void RegistReq_ItemPickUp(Action<int, FrameItemPickReqMsg> action)
        {
            _server.AddRegister<FrameItemPickReqMsg>(action);
        }

    }

}
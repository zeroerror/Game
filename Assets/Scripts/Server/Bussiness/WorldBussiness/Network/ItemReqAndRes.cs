using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.World;
using Game.Client.Bussiness.WorldBussiness;
using Game.Client.Bussiness.WorldBussiness.Interface;

namespace Game.Server.Bussiness.WorldBussiness.Network
{

    public class ItemReqAndRes
    {
        NetworkServer _server;

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
        }

        public void SendRes_ItemSpawn(int connId, int frameIndex, byte[] itemTypeArray,byte[] subtypeArray ,ushort[] entityIdArray)
        {
            FrameItemSpawnResMsg msg = new FrameItemSpawnResMsg
            {
                serverFrame = frameIndex,
                itemTypeArray = itemTypeArray,
                subtypeArray = subtypeArray,
                entityIdArray = entityIdArray
            };

            _server.SendMsg(connId, msg);
        }

        // ====== Regist ======
        public void RegistReq_ItemPickUp(Action<int, FrameItemPickReqMsg> action)
        {
            _server.AddRegister<FrameItemPickReqMsg>(action);
        }

    }

}
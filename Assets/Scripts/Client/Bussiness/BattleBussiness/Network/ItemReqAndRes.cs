using System;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class ItemReqAndRes
    {
        NetworkClient _client;

        public ItemReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            _client = client;
        }

        // ====== Send ======
        public void SendReq_ItemPickUp(int wRid, ItemType itemType, int entityId)
        {
            FrameItemPickReqMsg msg = new FrameItemPickReqMsg
            {
                wRid = (byte)wRid,
                itemType = (byte)itemType,
                entityId = (ushort)entityId
            };
            _client.SendMsg(msg);
            Debug.Log($"[wRid:{wRid}]请求拾取 {itemType.ToString()}物件[entityId:{entityId}]");
        }

        // ====== Regist ======
        public void RegistRes_ItemPickUp(Action<FrameItemPickResMsg> action)
        {
            _client.RegistMsg<FrameItemPickResMsg>(action);
        }

        public void RegistRes_ItemSpawn(Action<FrameItemSpawnResMsg> action)
        {
            _client.RegistMsg(action);
        }

    }
}
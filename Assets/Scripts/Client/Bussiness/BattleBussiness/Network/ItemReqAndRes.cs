using System;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.BattleBussiness.Generic;
using System.Collections.Generic;
using ZeroFrame.Protocol;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class ItemReqAndRes
    {
        NetworkClient battleClient;
        List<Action> actionList;
        object lockObj;

        public ItemReqAndRes()
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

        // ====== Send ======
        public void SendReq_ItemPickUp(int roleEntityID, EntityType entityType, int entityId)
        {
            FrameItemPickReqMsg msg = new FrameItemPickReqMsg
            {
                roleID = (byte)roleEntityID,
                entityType = (byte)entityType,
                itemID = (ushort)entityId
            };
            battleClient.SendMsg(msg);
            Debug.Log($"[wRid:{roleEntityID}]请求拾取 {entityType.ToString()}物件[entityId:{entityId}]");
        }

        // ====== Regist ======
        public void RegistRes_ItemPickUp(Action<FrameItemPickResMsg> action)
        {
            AddRegister(action);
        }

        public void RegistRes_ItemSpawn(Action<FrameItemSpawnResMsg> action)
        {
            AddRegister(action);
        }

        // Private Func
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
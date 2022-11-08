

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

        public void SendRes_EntitySpawn(int connID, EntityType entityType, byte subType, int entityID, Vector3 pos)
        {
            BattleEntitySpawnResMsg msg = new BattleEntitySpawnResMsg();
            msg.serverFrame = serverFrame;
            msg.entityType = (byte)entityType;
            msg.subType = subType;
            msg.entityID = entityID;
            msg.posX = (int)(pos.x * 10000f);
            msg.posY = (int)(pos.y * 10000f);
            msg.posZ = (int)(pos.z * 10000f);

            battleServer.SendMsg(connID, msg);
            sendCount++;
            Debug.Log($"实体[{entityType.ToString()}]生成消息发送");
        }

        public void SendRes_EntityTearDown(int connId, EntityType entityType, int entityID, Vector3 pos)
        {
            BattleEntityTearDownResMsg msg = new BattleEntityTearDownResMsg();
            msg.serverFrame = serverFrame;
            msg.entityType = (byte)entityType;
            msg.entityID = entityID;
            msg.posX = (int)(pos.x * 10000f);
            msg.posY = (int)(pos.y * 10000f);
            msg.posZ = (int)(pos.z * 10000f);

            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log($"实体[{entityType.ToString()}]销毁消息发送");
        }

        public void SendRes_BattleGameStateAndStage(int connID, BattleState state, BattleStage stage, int curMaintainFrame)
        {
            BattleStateAndStageResMsg msg = new BattleStateAndStageResMsg();
            msg.state = (byte)state;
            msg.curMaintainFrame = curMaintainFrame;
            msg.stage = (int)stage;

            battleServer.SendMsg(connID, msg);
            sendCount++;
        }

        public void SendRes_BattleAirdropSpawn(int connID, EntityType airdropEntityType, byte subType, int entityID, Vector3 pos, BattleStage curLvStage)
        {
            BattleAirdropSpawnResMsg msg = new BattleAirdropSpawnResMsg();
            msg.airdropEntityType = (byte)airdropEntityType;
            msg.subType = subType;
            msg.entityID = entityID;
            msg.posX = (int)(pos.x * 10000f);
            msg.posY = (int)(pos.y * 10000f);
            msg.posZ = (int)(pos.z * 10000f);
            msg.curLvStage = (int)curLvStage;

            battleServer.SendMsg(connID, msg);
            sendCount++;
        }

        public void SendRes_BattleAirdropTearDown(int connID, EntityType airdropEntityType, byte subType, int entityID, Vector3 pos, BattleStage battleStage)
        {
            BattleAirdropTearDownResMsg msg = new BattleAirdropTearDownResMsg();
            msg.subType = subType;
            msg.entityID = entityID;

            battleServer.SendMsg(connID, msg);
            sendCount++;
        }

        // ====== Regist ======
        public void RegistReq_BattleGameStateAndStage(Action<int, BattleStateAndStageReqMsg> action)
        {
            AddRegister(action);
        }

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
using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.Battle;
using Game.Client.Bussiness.BattleBussiness;
using System.Collections.Generic;
using ZeroFrame.Protocol;

namespace Game.Server.Bussiness.BattleBussiness.Network
{

    public class WeaponReqAndRes
    {
        NetworkServer battleServer;
        int serverFrame;
        public void SetServerFrame(int serveFrame) => this.serverFrame = serveFrame;

        int sendCount;
        public int SendCount => sendCount;
        public void ClearSendCount() => sendCount = 0;

        List<Action> actionList;

        public WeaponReqAndRes()
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

        #region [Send]

        public void SendRes_WeaponShoot(int connId, byte masterId)
        {
            FrameWeaponFireResMsg msg = new FrameWeaponFireResMsg
            {
                masterId = masterId,
            };
            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log("回复武器射击请求");
        }

        public void SendRes_WeaponReloaded(int connId, int serverFrame, int masterId, int reloadBulletNum)
        {
            FrameWeaponReloadResMsg msg = new FrameWeaponReloadResMsg
            {
                masterId = (byte)masterId,
                reloadBulletNum = (byte)reloadBulletNum
            };
            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log("回复武器装弹请求");
        }

        public void SendRes_WeaponDrop(int connId, int serverFrame, int masterId, ushort entityId)
        {
            FrameWeaponDropResMsg msg = new FrameWeaponDropResMsg
            {
                entityId = entityId,
                masterId = (byte)masterId,
            };
            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log("回复武器丢弃请求");
        }

        #endregion

        #region [Regist]

        public void RegistReq_WeaponShoot(Action<int, FrameWeaponFireReqMsg> action)
        {
            AddRegister(action);
        }

        public void RegistReq_WeaponReload(Action<int, FrameWeaponReloadReqMsg> action)
        {
            AddRegister(action);
        }

        public void RegistReq_WeaponDrop(Action<int, FrameWeaponDropReqMsg> action)
        {
            AddRegister(action);
        }

        #endregion

        // Private Func
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
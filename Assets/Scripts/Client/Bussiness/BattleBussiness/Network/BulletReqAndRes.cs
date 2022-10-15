using System;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.BattleBussiness.Generic;
using System.Collections.Generic;
using ZeroFrame.Protocol;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class BulletReqAndRes
    {
        NetworkClient battleClient;
        List<Action> actionList;

        public BulletReqAndRes()
        {
            actionList = new List<Action>();
        }

        public void Inject(NetworkClient client)
        {
            battleClient = client;
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

        public void SendReq_BulletSpawn(BulletType bulletType, byte wRid, Vector3 targetPos)
        {
            targetPos *= 10000f;

            FrameBulletSpawnReqMsg msg = new FrameBulletSpawnReqMsg
            {
                bulletType = (byte)bulletType,
                wRid = wRid,
                targetPosX = (int)(targetPos.x),
                targetPosY = (int)(targetPos.y),
                targetPosZ = (int)(targetPos.z)
            };
            battleClient.SendMsg(msg);
            Debug.Log($"发送生成子弹网络请求:wRid:{wRid} 目标点：{targetPos}");
        }


        #region [Regist]

        public void RegistRes_BulletSpawn(Action<FrameBulletSpawnResMsg> action)
        {
            AddRegister(action);
        }

        public void RegistRes_BulletTearDown(Action<FrameBulletLifeOverResMsg> action)
        {
            AddRegister(action);
        }

        public void RegistRes_BulletHitRole(Action<FrameBulletHitRoleResMsg> action)
        {
            AddRegister(action);
        }

        public void RegistRes_BulletHitWall(Action<FrameBulletHitWallResMsg> action)
        {
            AddRegister(action);
        }

        #endregion

        // Private Func
        void AddRegister<T>(Action<T> action) where T : IZeroMessage<T>, new()
        {
            lock (actionList)
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
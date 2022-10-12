using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.Battle;
using Game.Client.Bussiness.BattleBussiness;
using Game.Client.Bussiness.BattleBussiness.Generic;
using ZeroFrame.Protocol;
using System.Collections.Generic;

namespace Game.Server.Bussiness.BattleBussiness.Network
{

    public class BulletReqAndRes
    {
        NetworkServer battleServer;
        int serverFrame;
        public void SetServerFrame(int serveFrame) => this.serverFrame = serveFrame;

        int sendCount;
        public int SendCount => sendCount;
        public void ClearSendCount() => sendCount = 0;

        List<Action> actionList;

        public BulletReqAndRes()
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
            lock (actionList)
            {
                actionList.ForEach((action) =>
                       {
                           action.Invoke();
                       });
                actionList.Clear();
            }
        }

        #region [Send]

        public void SendRes_BulletSpawn(int connId, BulletType bulletType, ushort bulletId, byte wRid, Vector3 dir)
        {
            FrameBulletSpawnResMsg msg = new FrameBulletSpawnResMsg
            {
                serverFrame = serverFrame,
                wRid = wRid,
                bulletType = (byte)bulletType,
                bulletId = bulletId,
                shootDirX = (short)(dir.x * 100),
                shootDirY = (short)(dir.y * 100),
                shootDirZ = (short)(dir.z * 100),
            };

            battleServer.SendMsg(connId, msg);
            // Debug.Log($"dir.z:{dir.z} shootDirZ :{msg.shootDirZ}");
            sendCount++;
        }

        public void SendRes_BulletHitRole(int connId, ushort bulletId, int entityId)
        {
            FrameBulletHitRoleResMsg msg = new FrameBulletHitRoleResMsg
            {
                serverFrame = serverFrame,
                bulletId = bulletId,
                entityId = (byte)entityId
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log($"发送子弹击中角色消息 wRid {entityId}");
        }

        public void SendRes_BulletHitField(int connId, BulletEntity bulletEntity)
        {
            var bulletPos = bulletEntity.MoveComponent.CurPos;
            bulletPos *= 10000f;
            FrameBulletHitWallResMsg msg = new FrameBulletHitWallResMsg
            {
                serverFrame = serverFrame,
                bulletId = bulletEntity.EntityId,
                posX = (int)bulletPos.x,
                posY = (int)bulletPos.y,
                posZ = (int)bulletPos.z,
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log($"发送子弹击中墙壁消息 ");
        }

        public void SendRes_BulletTearDown(int connId, BulletType bulletType, byte wRid, ushort bulletId, Vector3 pos)
        {
            Debug.Log($"子弹销毁消息发送: serverFrame：{serverFrame} wRid：{wRid}");
            FrameBulletTearDownResMsg msg = new FrameBulletTearDownResMsg
            {
                serverFrame = serverFrame,
                bulletType = (byte)bulletType,
                wRid = wRid,
                bulletId = bulletId,
                posX = (int)(pos.x * 10000f),  // (16 16) 整数部16位 short -32768 --- +32767 小数部分16位 ushort(0 --- +65535) 0.0000到0.9999
                posY = (int)(pos.y * 10000f),
                posZ = (int)(pos.z * 10000f)
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
        }

        #endregion

        #region [Regist]

        public void RegistReq_BulletSpawn(Action<int, FrameBulletSpawnReqMsg> action)
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
                    action.Invoke(connId, msg);
                });
            });
        }

    }

}
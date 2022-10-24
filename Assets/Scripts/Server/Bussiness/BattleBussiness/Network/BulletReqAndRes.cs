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
            for (int i = 0; i < actionList.Count; i++)
            {
                var action = actionList[i];
                action?.Invoke();
            }
            actionList.Clear();
        }

        #region [Send]

        public void SendRes_BulletSpawn(int connId, BulletEntity bulletEntity)
        {
            var bulletPos = bulletEntity.MoveComponent.Position;
            var fireDir = bulletEntity.MoveComponent.GetFaceDir();

            FrameBulletSpawnResMsg msg = new FrameBulletSpawnResMsg
            {
                serverFrame = serverFrame,
                bulletType = (byte)bulletEntity.BulletType,
                masterEntityId = (byte)bulletEntity.MasterEntityId,
                bulletEntityId = (ushort)bulletEntity.IDComponent.EntityId,
                startPosX = (int)(bulletPos.x * 10000),
                startPosY = (int)(bulletPos.y * 10000),
                startPosZ = (int)(bulletPos.z * 10000),
                fireDirX = (short)(fireDir.x * 10000),
                fireDirZ = (short)(fireDir.z * 10000),
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
            
        }

        public void SendRes_BulletHitRole(int connId, int bulletEntityId, int roleEntityId)
        {
            FrameBulletHitRoleResMsg msg = new FrameBulletHitRoleResMsg
            {
                serverFrame = serverFrame,
                bulletEntityId = (ushort)bulletEntityId,
                roleEntityId = (byte)roleEntityId
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log($"发送子弹击中角色消息 wRid {roleEntityId}");
        }

        public void SendRes_BulletHitField(int connId, BulletEntity bulletEntity)
        {
            var bulletPos = bulletEntity.MoveComponent.Position;
            bulletPos *= 10000f;
            FrameBulletHitFieldResMsg msg = new FrameBulletHitFieldResMsg
            {
                serverFrame = serverFrame,
                bulletEntityID = (ushort)bulletEntity.IDComponent.EntityId,
                posX = (int)bulletPos.x,
                posY = (int)bulletPos.y,
                posZ = (int)bulletPos.z,
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log($"发送子弹击中墙壁消息 ");
        }

        public void SendRes_BulletLifeFrameOver(int connId, BulletEntity bulletEntity)
        {
            BulletType bulletType = bulletEntity.BulletType;
            int masterEntityID = bulletEntity.MasterEntityId;
            int bulletEntityID = bulletEntity.IDComponent.EntityId;
            Vector3 pos = bulletEntity.MoveComponent.Position;
            Debug.Log($"子弹销毁消息发送: serverFrame：{serverFrame} wRid：{masterEntityID}");
            FrameBulletLifeOverResMsg msg = new FrameBulletLifeOverResMsg
            {
                serverFrame = serverFrame,
                bulletType = (byte)bulletType,
                wRid = (byte)masterEntityID,
                bulletId = (ushort)bulletEntityID,
                posX = (int)(pos.x * 10000f),  // (16 16) 整数部16位 short -32768 --- +32767 小数部分16位 ushort(0 --- +65535) 0.0000到0.9999
                posY = (int)(pos.y * 10000f),
                posZ = (int)(pos.z * 10000f)
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
        }

        #endregion

        #region [Regist]

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
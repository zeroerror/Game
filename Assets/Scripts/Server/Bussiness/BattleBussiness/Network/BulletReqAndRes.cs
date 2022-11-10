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
            var bulletPos = bulletEntity.LocomotionComponent.Position;
            var fireDir = bulletEntity.LocomotionComponent.GetFaceDir();

            BattleBulletSpawnResMsg msg = new BattleBulletSpawnResMsg
            {
                serverFrame = serverFrame,
                bulletType = (byte)bulletEntity.BulletType,
                weaponID = (byte)bulletEntity.WeaponID,
                bulletID = (ushort)bulletEntity.IDComponent.EntityID,
                startPosX = (int)(bulletPos.x * 10000),
                startPosY = (int)(bulletPos.y * 10000),
                startPosZ = (int)(bulletPos.z * 10000),
                fireDirX = (short)(fireDir.x * 10000),
                fireDirZ = (short)(fireDir.z * 10000),
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
        }

        public void SendRes_BulletHitEntity(int connId, int bulletEntityId, int entityID, EntityType entityType)
        {
            BattleBulletHitEntityResMsg msg = new BattleBulletHitEntityResMsg
            {
                serverFrame = serverFrame,
                bulletEntityID = (ushort)bulletEntityId,
                entityID = (byte)entityID,
                entityType = (byte)entityType
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log($"发送: 子弹击中Entity {entityType},{entityID} 消息");
        }

        public void SendRes_BulletHitField(int connId, BulletEntity bulletEntity)
        {
            var bulletPos = bulletEntity.LocomotionComponent.Position;
            bulletPos *= 10000f;
            BattleBulletHitFieldResMsg msg = new BattleBulletHitFieldResMsg
            {
                serverFrame = serverFrame,
                bulletEntityID = (ushort)bulletEntity.IDComponent.EntityID,
                posX = (int)bulletPos.x,
                posY = (int)bulletPos.y,
                posZ = (int)bulletPos.z,
            };

            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log($"发送: 子弹击中墙壁 ");
        }

        public void SendRes_BulletLifeTimeOver(int connId, int entityID)
        {
            BattleBulletLifeTimeOverResMsg msg = new BattleBulletLifeTimeOverResMsg();
            msg.entityID = entityID;

            battleServer.SendMsg(connId, msg);
            sendCount++;
            Debug.Log($"发送: 子弹生命周期结束");
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
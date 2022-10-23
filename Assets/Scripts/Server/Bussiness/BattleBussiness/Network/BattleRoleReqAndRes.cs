using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.Protocol;
using Game.Infrastructure.Network.Server;
using Game.Protocol.Battle;
using Game.Client.Bussiness.BattleBussiness;
using Game.Generic;

namespace Game.Server.Bussiness.BattleBussiness.Network
{

    public class BattleRoleReqAndRes
    {
        NetworkServer battleServer;
        int serverFrame;
        public void SetServerFrame(int serveFrame) => this.serverFrame = serveFrame;

        int sendCount;
        public int SendCount => sendCount;
        public void ClearSendCount() => sendCount = 0;

        List<Action> actionList;

        public BattleRoleReqAndRes()
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
                action?.Invoke();
            }
            actionList.Clear();
        }

        #region [Send]
        public void SendUpdate_RoleState(int connId, BattleRoleLogicEntity role)
        {
            // Position
            var pos = role.MoveComponent.Position;
            int x = (int)(pos.x * 10000);
            int y = (int)(pos.y * 10000);
            int z = (int)(pos.z * 10000);

            // Rotation
            var rot = role.transform.rotation;
            var eulerAngle = rot.eulerAngles;
            int rotX = (int)(eulerAngle.x * 10000);
            int rotY = (int)(eulerAngle.y * 10000);
            int rotZ = (int)(eulerAngle.z * 10000);

            // Velocity
            var moveVelocity = role.MoveComponent.MoveVelocity;
            int moveVelocityX = (int)(moveVelocity.x * 10000);
            int moveVelocityY = (int)(moveVelocity.y * 10000);
            int moveVelocityZ = (int)(moveVelocity.z * 10000);

            var extraVelocity = role.MoveComponent.ExtraVelocity;
            int extraVelocityX = (int)(extraVelocity.x * 10000);
            int extraVelocityY = (int)(extraVelocity.y * 10000);
            int extraVelocityZ = (int)(extraVelocity.z * 10000);

            int gravityVelocity = (int)(role.MoveComponent.GravityVelocity * 10000);

            // DebugExtensions.LogWithColor($"发送状态同步帧{serverFrame} connId:{connId} wRid:{role.IDComponent.EntityId}  位置 :{pos} 移动速度：{moveVelocity} 额外速度：{extraVelocity}  重力速度:{role.MoveComponent.GravityVelocity}  旋转角度：{eulerAngle}", "#008000");

            BattleRoleSyncMsg msg = new BattleRoleSyncMsg
            {
                serverFrame = serverFrame,
                entityId = (byte)role.IDComponent.EntityId,
                roleState = (int)role.StateComponent.RoleState,
                x = x,
                y = y,
                z = z,
                eulerX = rotX,
                eulerY = rotY,
                eulerZ = rotZ,
                moveVelocityX = moveVelocityX,
                moveVelocityY = moveVelocityY,
                moveVelocityZ = moveVelocityZ,
                extraVelocityX = extraVelocityX,
                extraVelocityY = extraVelocityY,
                extraVelocityZ = extraVelocityZ,
                gravityVelocity = gravityVelocity,
                isOwner = (connId == role.ConnId)
            };
            battleServer.SendMsg<BattleRoleSyncMsg>(connId, msg);
            sendCount++;
        }

        public void SendRes_BattleRoleSpawn(int connId, int entityID, bool isOwner)
        {
            FrameBattleRoleSpawnResMsg frameResWRoleSpawnMsg = new FrameBattleRoleSpawnResMsg
            {
                serverFrame = serverFrame,
                entityId = (byte)entityID,
                isOwner = isOwner
            };
            battleServer.SendMsg<FrameBattleRoleSpawnResMsg>(connId, frameResWRoleSpawnMsg);
            Debug.Log($"服务端回复帧消息 serverFrame:{serverFrame} connId:{connId} ---->确认人物生成");
            sendCount++;
        }

        #endregion

        #region [Regist]

        // == OPT ==
        public void RegistReq_RoleMove(Action<int, FrameRoleMoveReqMsg> action)
        {
            AddRegister(action);
        }

        public void RegistReq_RoleRotate(Action<int, FrameRoleRotateReqMsg> action)
        {
            AddRegister(action);
        }

        public void RegistReq_Jump(Action<int, FrameRollReqMsg> action)
        {
            AddRegister(action);
        }

        // == SPAWN ==
        public void RegistReq_BattleRoleSpawn(Action<int, FrameBattleRoleSpawnReqMsg> action)
        {
            AddRegister(action);
        }

        #endregion

        // Private Func
        void AddRegister<T>(Action<int, T> action) where T : IZeroMessage<T>, new()
        {
            lock (actionList)
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

}
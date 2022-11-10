using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.Protocol;
using Game.Infrastructure.Network.Server;
using Game.Protocol.Battle;
using Game.Client.Bussiness.BattleBussiness;
using Game.Generic;
using Game.Client.Bussiness.BattleBussiness.Generic;

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
            var pos = role.LocomotionComponent.Position;
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
            var velocity = role.LocomotionComponent.Velocity;
            int velocityX = (int)(velocity.x * 10000);
            int velocityY = (int)(velocity.y * 10000);
            int velocityZ = (int)(velocity.z * 10000);

            // DebugExtensions.LogWithColor($"发送状态同步帧{serverFrame} connId:{connId} wRid:{role.IDComponent.EntityId}  位置 :{pos} 移动速度：{moveVelocity} 额外速度：{extraVelocity}  重力速度:{role.MoveComponent.GravityVelocity}  旋转角度：{eulerAngle}", "#008000");

            BattleRoleSyncMsg msg = new BattleRoleSyncMsg
            {
                serverFrame = serverFrame,
                entityId = (byte)role.IDComponent.EntityID,
                roleState = (int)role.StateComponent.RoleState,
                posX = x,
                posY = y,
                posZ = z,
                eulerX = rotX,
                eulerY = rotY,
                eulerZ = rotZ,
                velocityX = velocityX,
                velocityY = velocityY,
                velocityZ = velocityZ,
            };
            battleServer.SendMsg<BattleRoleSyncMsg>(connId, msg);
            sendCount++;
        }

        public void SendRes_BattleRoleSpawn(int connId, int entityID, byte controlType)
        {
            BattleRoleSpawnResMsg frameResWRoleSpawnMsg = new BattleRoleSpawnResMsg
            {
                serverFrame = serverFrame,
                entityId = (byte)entityID,
                controlType = controlType
            };
            battleServer.SendMsg<BattleRoleSpawnResMsg>(connId, frameResWRoleSpawnMsg);
            Debug.Log($"SendRes_BattleRoleSpawn entityID:{entityID} controlType:{controlType} ---->人物生成");
            sendCount++;
        }

        #endregion

        #region [Regist]

        // == OPT ==
        public void RegistReq_RoleMove(Action<int, BattleRoleMoveReqMsg> action)
        {
            AddRegister(action);
        }

        public void RegistReq_RoleRotate(Action<int, BattleRoleRotateReqMsg> action)
        {
            AddRegister(action);
        }

        public void RegistReq_Jump(Action<int, BattleRoleRollReqMsg> action)
        {
            AddRegister(action);
        }

        // == SPAWN ==
        public void RegistReq_BattleRoleSpawn(Action<int, BattleRoleSpawnReqMsg> action)
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
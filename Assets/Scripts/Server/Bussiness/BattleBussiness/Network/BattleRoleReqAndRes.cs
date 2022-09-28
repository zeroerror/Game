using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.Client2World;
using Game.Protocol.Battle;
using Game.Client.Bussiness.BattleBussiness;
using Game.Generic;

namespace Game.Server.Bussiness.BattleBussiness.Network
{

    public class BattleRoleReqAndRes
    {
        NetworkServer _server;
        int serverFrame;
        public void SetServerFrame(int serveFrame) => this.serverFrame = serveFrame;

        int sendCount;
        public int SendCount => sendCount;
        public void ClearSendCount() => sendCount = 0;

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        // == Send ==
        public void SendUpdate_WRoleState(int connId, BattleRoleLogicEntity role)
        {
            // Position
            var pos = role.MoveComponent.CurPos;
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

            // DebugExtensions.LogWithColor($"发送状态同步帧{serverFrame} connId:{connId} wRid:{role.EntityId} 角色状态:{role.RoleState.ToString()} 位置 :{pos} 移动速度：{moveVelocity} 额外速度：{extraVelocity}  重力速度:{role.MoveComponent.GravityVelocity}  旋转角度：{eulerAngle}", "#008000");

            BattleRoleStateUpdateMsg msg = new BattleRoleStateUpdateMsg
            {
                serverFrameIndex = serverFrame,
                wRid = role.EntityId,
                roleState = (int)role.RoleState,
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
                isOwner = connId == role.ConnId
            };
            _server.SendMsg<BattleRoleStateUpdateMsg>(connId, msg);
            sendCount++;
        }

        public void SendRes_BattleRoleSpawn(int connId, byte wRoleId, bool isOwner)
        {
            FrameBattleRoleSpawnResMsg frameResWRoleSpawnMsg = new FrameBattleRoleSpawnResMsg
            {
                serverFrame = serverFrame,
                wRoleId = wRoleId,
                isOwner = isOwner
            };
            _server.SendMsg<FrameBattleRoleSpawnResMsg>(connId, frameResWRoleSpawnMsg);
            Debug.Log($"服务端回复帧消息 serverFrame:{serverFrame} connId:{connId} ---->确认人物生成");
            sendCount++;
        }

        // == Regist ==
        // == OPT ==
        public void RegistReq_RoleMove(Action<int, FrameRoleMoveReqMsg> action)
        {
            _server.AddRegister(action);
        }

        public void RegistReq_RoleRotate(Action<int, FrameRoleRotateReqMsg> action)
        {
            _server.AddRegister(action);
        }

        public void RegistReq_Jump(Action<int, FrameJumpReqMsg> action)
        {
            _server.AddRegister(action);
        }

        // == SPAWN ==
        public void RegistReq_BattleRoleSpawn(Action<int, FrameBattleRoleSpawnReqMsg> action)
        {
            _server.AddRegister(action);
        }

    }

}
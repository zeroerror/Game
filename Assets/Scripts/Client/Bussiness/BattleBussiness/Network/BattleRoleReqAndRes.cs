using System;
using UnityEngine;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Battle;
using System.Runtime.InteropServices;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class BattleRoleReqAndRes
    {
        NetworkClient battleClient;

        int clientFrame;
        public void SetClientFrame(int clienFrame) => this.clientFrame = clienFrame;

        public BattleRoleReqAndRes()
        {
        }

        public void Inject(NetworkClient client)
        {
            battleClient = client;
        }

        // == Send ==
        public void SendReq_RoleMove(int rid, Vector3 dir)
        {
            var d = dir;
            dir.Normalize();
            ulong msg = (ulong)(ushort)rid << 48;     //16 wrid 16 x 16 y 16 z
            msg |= (ulong)(ushort)(int)(dir.x * 100) << 32;
            msg |= (ulong)(ushort)(int)(dir.y * 100) << 16;
            msg |= (ulong)(ushort)(int)(dir.z * 100);
            FrameRoleMoveReqMsg frameRoleMoveReqMsg = new FrameRoleMoveReqMsg
            {
                msg = msg
            };
            battleClient.SendMsg(frameRoleMoveReqMsg);
        }

        public void SendReq_RoleRotate(BattleRoleLogicEntity roleEntity)
        {
            var eulerAngel = roleEntity.transform.rotation.eulerAngles;
            var rid = roleEntity.IDComponent.EntityId;
            ulong msg = (ulong)(ushort)rid << 48;     //16 wrid 16 x 16 y 16 z
            msg |= (ulong)(ushort)eulerAngel.x << 32;
            msg |= (ulong)(ushort)eulerAngel.y << 16;
            msg |= (ulong)(ushort)eulerAngel.z;

            FrameRoleRotateReqMsg frameRoleRotateReqMsg = new FrameRoleRotateReqMsg
            {
                msg = msg
            };
            battleClient.SendMsg(frameRoleRotateReqMsg);
        }

        public void SendReq_RoleJump(BattleRoleLogicEntity roleLogicEntity)
        {
            Vector3 dir = roleLogicEntity.transform.forward;
            int dirX = (int)(dir.x * 10000f);
            int dirY = (int)(dir.y * 10000f);
            int dirZ = (int)(dir.z * 10000f);

            FrameJumpReqMsg frameJumpReqMsg = new FrameJumpReqMsg
            {
                entityId = (byte)roleLogicEntity.IDComponent.EntityId,
                dirX = dirX,
                dirY = dirY,
                dirZ = dirZ
            };

            battleClient.SendMsg(frameJumpReqMsg);
        }

        public void SendReq_BattleRoleSpawn()
        {
            FrameBattleRoleSpawnReqMsg frameReqWRoleSpawnMsg = new FrameBattleRoleSpawnReqMsg
            {
            };
            battleClient.SendMsg(frameReqWRoleSpawnMsg);
        }

        // == Regist ==

        public void RegistRes_BattleRoleSpawn(Action<FrameBattleRoleSpawnResMsg> action)
        {
            battleClient.RegistMsg(action);
        }

        public void RegistUpdate_WRole(Action<BattleRoleStateUpdateMsg> action)
        {
            battleClient.RegistMsg(action);
        }


    }

}
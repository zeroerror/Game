using System;
using UnityEngine;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Battle;
using System.Runtime.InteropServices;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    [StructLayout(LayoutKind.Explicit)]
    public struct FloatContent {
        [FieldOffset(0)]
        public float fvalue;
        [FieldOffset(0)]
        public int ivalue;
    }

    public class BattleRoleReqAndRes
    {
        NetworkClient _client;

        int clientFrame;
        public void SetClientFrame(int clienFrame) => this.clientFrame = clienFrame;

        public BattleRoleReqAndRes()
        {
            float x = -50;
            Debug.Log((ulong)(ushort)(uint)(x));


        }

        public void Inject(NetworkClient client)
        {
            _client = client;
        }

        // == Send ==
        public void SendReq_RoleMove(byte rid, Vector3 dir)
        {
            dir.Normalize();
            Debug.Log($"DIR:{dir}");
            ulong msg = (ulong)(ushort)rid << 48;     //16 wrid 16 x 16 y 16 z
            msg |= (ulong)(ushort)(uint)(dir.x * 100) << 32;
            msg |= (ulong)(ushort)(dir.y * 100) << 16;
            msg |= (ulong)(ushort)(dir.z * 100);
            FrameRoleMoveReqMsg frameRoleMoveReqMsg = new FrameRoleMoveReqMsg
            {
                msg = msg
            };
            _client.SendMsg(frameRoleMoveReqMsg);
        }

        public void SendReq_RoleRotate(BattleRoleLogicEntity roleEntity)
        {
            var eulerAngel = roleEntity.transform.rotation.eulerAngles;
            var rid = roleEntity.EntityId;
            ulong msg = (ulong)(ushort)rid << 48;     //16 wrid 16 x 16 y 16 z
            msg |= (ulong)(ushort)eulerAngel.x << 32;
            msg |= (ulong)(ushort)eulerAngel.y << 16;
            msg |= (ulong)(ushort)eulerAngel.z;

            FrameRoleRotateReqMsg frameRoleRotateReqMsg = new FrameRoleRotateReqMsg
            {
                msg = msg
            };
            _client.SendMsg(frameRoleRotateReqMsg);
        }

        public void SendReq_RoleJump(byte wRid)
        {
            FrameJumpReqMsg frameJumpReqMsg = new FrameJumpReqMsg
            {
                wRid = wRid
            };

            _client.SendMsg(frameJumpReqMsg);
        }

        public void SendReq_BattleRoleSpawn()
        {
            FrameBattleRoleSpawnReqMsg frameReqWRoleSpawnMsg = new FrameBattleRoleSpawnReqMsg
            {
            };
            _client.SendMsg(frameReqWRoleSpawnMsg);
        }

        // == Regist ==

        public void RegistRes_BattleRoleSpawn(Action<FrameBattleRoleSpawnResMsg> action)
        {
            _client.RegistMsg<FrameBattleRoleSpawnResMsg>(action);
        }

        public void RegistUpdate_WRole(Action<BattleRoleStateUpdateMsg> action)
        {
            _client.RegistMsg<BattleRoleStateUpdateMsg>(action);
        }


    }

}
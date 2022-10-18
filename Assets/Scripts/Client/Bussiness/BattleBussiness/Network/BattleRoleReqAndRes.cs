using System;
using UnityEngine;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Battle;
using System.Runtime.InteropServices;
using Game.Client.Bussiness.EventCenter;
using ZeroFrame.Protocol;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class BattleRoleReqAndRes
    {
        NetworkClient battleClient;
        List<Action> actionList;
        object lockObj;

        int clientFrame;
        public void SetClientFrame(int clienFrame) => this.clientFrame = clienFrame;

        public BattleRoleReqAndRes()
        {
            actionList = new List<Action>();
        }

        public void Inject(NetworkClient client)
        {
            battleClient = client;
        }

        public void TickAllRegistAction()
        {
            lock (lockObj)
            {
                for (int i = 0; i < actionList.Count; i++)
                {
                    var action = actionList[i];
                    action.Invoke();
                }
                actionList.Clear();
            }
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

        public void SendReq_RoleRoll(BattleRoleLogicEntity role)
        {
            Vector3 dir = role.transform.forward;
            int dirX = (int)(dir.x * 10000f);
            int dirY = (int)(dir.y * 10000f);
            int dirZ = (int)(dir.z * 10000f);

            FrameRollReqMsg frameJumpReqMsg = new FrameRollReqMsg
            {
                entityId = (byte)role.IDComponent.EntityId,
                dirX = dirX,
                dirY = dirY,
                dirZ = dirZ
            };

            battleClient.SendMsg(frameJumpReqMsg);
            Debug.Log($"SendReq_RoleRoll dir:{dir} ");
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
            AddRegister(action);
        }

        public void RegistUpdate_WRole(Action<BattleRoleStateUpdateMsg> action)
        {
            AddRegister(action);
        }

        // Private Func
        void AddRegister<T>(Action<T> action) where T : IZeroMessage<T>, new()
        {
            lock (lockObj)
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
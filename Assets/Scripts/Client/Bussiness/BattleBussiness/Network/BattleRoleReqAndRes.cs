using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.Protocol;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Battle;
using Game.Client.Bussiness.BattleBussiness.Generic;

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
            lockObj = new object();
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
                    action?.Invoke();
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
            BattleRoleMoveReqMsg frameRoleMoveReqMsg = new BattleRoleMoveReqMsg
            {
                msg = msg
            };
            battleClient.SendMsg(frameRoleMoveReqMsg);
        }

        public void SendReq_RoleRotate(BattleRoleLogicEntity roleEntity)
        {
            var eulerAngel = roleEntity.transform.rotation.eulerAngles;
            var rid = roleEntity.IDComponent.EntityID;
            ulong msg = (ulong)(ushort)rid << 48;     //16 wrid 16 x 16 y 16 z
            msg |= (ulong)(ushort)eulerAngel.x << 32;
            msg |= (ulong)(ushort)eulerAngel.y << 16;
            msg |= (ulong)(ushort)eulerAngel.z;

            BattleRoleRotateReqMsg frameRoleRotateReqMsg = new BattleRoleRotateReqMsg
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

            BattleRoleRollReqMsg frameJumpReqMsg = new BattleRoleRollReqMsg
            {
                entityId = (byte)role.IDComponent.EntityID,
                dirX = dirX,
                dirY = dirY,
                dirZ = dirZ
            };

            battleClient.SendMsg(frameJumpReqMsg);
        }

        public void SendReq_RoleSpawn(ControlType controlType)
        {
            BattleRoleSpawnReqMsg frameReqWRoleSpawnMsg = new BattleRoleSpawnReqMsg
            {
                controlType = (byte)controlType,
            };
            battleClient.SendMsg(frameReqWRoleSpawnMsg);
        }

        // == Regist ==

        public void RegistRes_BattleRoleSpawn(Action<BattleRoleSpawnResMsg> action)
        {
            AddRegister(action);
        }

        public void RegistUpdate_WRole(Action<BattleRoleSyncMsg> action)
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
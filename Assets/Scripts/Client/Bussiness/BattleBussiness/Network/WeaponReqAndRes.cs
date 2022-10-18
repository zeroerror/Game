using System;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Client;
using System.Collections.Generic;
using ZeroFrame.Protocol;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class WeaponReqAndRes
    {
        NetworkClient battleClient;
        List<Action> actionList;
        object lockObj;

        int clientFrame;
        public void SetClientFrame(int clientFrame) => this.clientFrame = clientFrame;

        public WeaponReqAndRes()
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

        // ====== Send ======
        public void SendReq_WeaponShoot(int masterId, Vector3 startPos, Vector3 endPos)
        {
            startPos *= 10000f;
            endPos *= 10000f;

            FrameWeaponShootReqMsg frameWeaponShootReqMsg = new FrameWeaponShootReqMsg
            {
                masterId = (byte)masterId,
                startPosX = (int)(startPos.x),
                startPosY = (int)(startPos.y),
                startPosZ = (int)(startPos.z),
                endPosX = (int)(endPos.x),
                endPosY = (int)(endPos.z),
                endPosZ = (int)(endPos.z),
            };
            battleClient.SendMsg(frameWeaponShootReqMsg);
        }

        public void SendReq_WeaponReload(BattleRoleLogicEntity role)
        {
            FrameWeaponReloadReqMsg frameWeaponReloadReqMsg = new FrameWeaponReloadReqMsg
            {
                masterId = role.IDComponent.EntityId,
            };
            battleClient.SendMsg(frameWeaponReloadReqMsg);
            Debug.Log("发送武器装弹请求");
        }

        public void SendReq_WeaponDrop(BattleRoleLogicEntity role)
        {
            if (role.WeaponComponent.CurrentWeapon == null) return;

            FrameWeaponDropReqMsg frameWeaponDropReqMsg = new FrameWeaponDropReqMsg
            {
                entityId = (ushort)role.WeaponComponent.CurrentWeapon.EntityId,
                masterId = role.IDComponent.EntityId
            };
            battleClient.SendMsg(frameWeaponDropReqMsg);
            Debug.Log("发送武器丢弃请求");
        }

        // ====== Regist ======
        public void RegistRes_WeaponShoot(Action<FrameWeaponShootResMsg> action)
        {
            AddRegister(action);
        }

        public void RegistRes_WeaponReload(Action<FrameWeaponReloadResMsg> action)
        {
            AddRegister(action);
        }

        public void RegistRes_WeaponDrop(Action<FrameWeaponDropResMsg> action)
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
using ZeroFrame.Protocol;
using Game.Protocol.Login;
using Game.Protocol.Client2World;
using Game.Protocol.Battle;
using Game.Protocol.World;
using System;using System.Collections.Generic;namespace Game.Protocol
{

    public class ProtocolService
    {
Dictionary<Type, ushort> messageInfoDic;
        public ProtocolService()
        {
            this.messageInfoDic = new Dictionary<Type, ushort>();
            messageInfoDic.Add(typeof(BattleRoleSyncMsg), 0);
            messageInfoDic.Add(typeof(FrameBattleRoleSpawnReqMsg), 1);
            messageInfoDic.Add(typeof(FrameBattleRoleSpawnResMsg), 2);
            messageInfoDic.Add(typeof(FrameBulletHitRoleResMsg), 3);
            messageInfoDic.Add(typeof(FrameBulletHitFieldResMsg), 4);
            messageInfoDic.Add(typeof(FrameBulletLifeOverResMsg), 5);
            messageInfoDic.Add(typeof(FrameBulletSpawnResMsg), 6);
            messageInfoDic.Add(typeof(FrameItemPickReqMsg), 7);
            messageInfoDic.Add(typeof(FrameItemPickResMsg), 8);
            messageInfoDic.Add(typeof(FrameItemSpawnResMsg), 9);
            messageInfoDic.Add(typeof(FrameRollReqMsg), 10);
            messageInfoDic.Add(typeof(FrameRoleMoveReqMsg), 11);
            messageInfoDic.Add(typeof(FrameRoleRotateReqMsg), 12);
            messageInfoDic.Add(typeof(FrameWeaponDropReqMsg), 13);
            messageInfoDic.Add(typeof(FrameWeaponDropResMsg), 14);
            messageInfoDic.Add(typeof(FrameWeaponShootReqMsg), 15);
            messageInfoDic.Add(typeof(FrameWeaponReloadReqMsg), 16);
            messageInfoDic.Add(typeof(FrameWeaponReloadResMsg), 17);
            messageInfoDic.Add(typeof(FrameWeaponShootResMsg), 18);
            messageInfoDic.Add(typeof(WolrdEnterReqMessage), 19);
            messageInfoDic.Add(typeof(WolrdEnterResMessage), 20);
            messageInfoDic.Add(typeof(LoginReqMessage), 21);
            messageInfoDic.Add(typeof(LoginResMessage), 22);
            messageInfoDic.Add(typeof(RegisterAccountReqMessage), 23);
            messageInfoDic.Add(typeof(RegisterAccountResMessage), 24);
            messageInfoDic.Add(typeof(WolrdLeaveReqMessage), 25);
            messageInfoDic.Add(typeof(WolrdLeaveResMessage), 26);
            messageInfoDic.Add(typeof(WorldRoomCreateReqMessage), 27);
            messageInfoDic.Add(typeof(WorldRoomCreateResMessage), 28);

        }

        public (byte serviceID, byte messageID) GetMessageID<T>()
        where T : IZeroMessage<T>
        {
            var type = typeof(T);
            bool hasMessage = messageInfoDic.TryGetValue(type, out ushort value);
            if (hasMessage)
            {
                return ((byte)value, (byte)(value >> 8));
            }
            else
            {
                throw new Exception();
            }
        }}

}
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
            messageInfoDic.Add(typeof(BattleGameStageUpdateResMsg), 0);
            messageInfoDic.Add(typeof(BattleRoleSyncMsg), 1);
            messageInfoDic.Add(typeof(FrameBattleRoleSpawnReqMsg), 2);
            messageInfoDic.Add(typeof(FrameBattleRoleSpawnResMsg), 3);
            messageInfoDic.Add(typeof(FrameBulletHitRoleResMsg), 4);
            messageInfoDic.Add(typeof(FrameBulletHitFieldResMsg), 5);
            messageInfoDic.Add(typeof(FrameBulletLifeOverResMsg), 6);
            messageInfoDic.Add(typeof(FrameBulletSpawnResMsg), 7);
            messageInfoDic.Add(typeof(FrameItemPickReqMsg), 8);
            messageInfoDic.Add(typeof(FrameItemPickResMsg), 9);
            messageInfoDic.Add(typeof(FrameItemSpawnResMsg), 10);
            messageInfoDic.Add(typeof(FrameRollReqMsg), 11);
            messageInfoDic.Add(typeof(FrameRoleMoveReqMsg), 12);
            messageInfoDic.Add(typeof(FrameRoleRotateReqMsg), 13);
            messageInfoDic.Add(typeof(FrameWeaponDropReqMsg), 14);
            messageInfoDic.Add(typeof(FrameWeaponDropResMsg), 15);
            messageInfoDic.Add(typeof(FrameWeaponShootReqMsg), 16);
            messageInfoDic.Add(typeof(FrameWeaponReloadReqMsg), 17);
            messageInfoDic.Add(typeof(FrameWeaponReloadResMsg), 18);
            messageInfoDic.Add(typeof(FrameWeaponShootResMsg), 19);
            messageInfoDic.Add(typeof(WolrdEnterReqMessage), 20);
            messageInfoDic.Add(typeof(WolrdEnterResMessage), 21);
            messageInfoDic.Add(typeof(LoginReqMessage), 22);
            messageInfoDic.Add(typeof(LoginResMessage), 23);
            messageInfoDic.Add(typeof(RegisterAccountReqMessage), 24);
            messageInfoDic.Add(typeof(RegisterAccountResMessage), 25);
            messageInfoDic.Add(typeof(WolrdLeaveReqMessage), 26);
            messageInfoDic.Add(typeof(WolrdLeaveResMessage), 27);
            messageInfoDic.Add(typeof(WorldRoomCreateReqMessage), 28);
            messageInfoDic.Add(typeof(WorldRoomCreateResMessage), 29);

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
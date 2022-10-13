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
            messageInfoDic.Add(typeof(BattleHeartbeatReqMsg), 0);
            messageInfoDic.Add(typeof(BattleHeartbeatResMsg), 1);
            messageInfoDic.Add(typeof(BattleRoleStateUpdateMsg), 2);
            messageInfoDic.Add(typeof(FrameBattleRoleSpawnReqMsg), 3);
            messageInfoDic.Add(typeof(FrameBattleRoleSpawnResMsg), 4);
            messageInfoDic.Add(typeof(FrameBulletHitRoleResMsg), 5);
            messageInfoDic.Add(typeof(FrameBulletHitWallResMsg), 6);
            messageInfoDic.Add(typeof(FrameBulletLifeOverResMsg), 7);
            messageInfoDic.Add(typeof(FrameBulletSpawnReqMsg), 8);
            messageInfoDic.Add(typeof(FrameBulletSpawnResMsg), 9);
            messageInfoDic.Add(typeof(FrameItemPickReqMsg), 10);
            messageInfoDic.Add(typeof(FrameItemPickResMsg), 11);
            messageInfoDic.Add(typeof(FrameItemSpawnResMsg), 12);
            messageInfoDic.Add(typeof(FrameJumpReqMsg), 13);
            messageInfoDic.Add(typeof(FrameRoleMoveReqMsg), 14);
            messageInfoDic.Add(typeof(FrameRoleRotateReqMsg), 15);
            messageInfoDic.Add(typeof(FrameWeaponDropReqMsg), 16);
            messageInfoDic.Add(typeof(FrameWeaponDropResMsg), 17);
            messageInfoDic.Add(typeof(FrameWeaponReloadReqMsg), 18);
            messageInfoDic.Add(typeof(FrameWeaponReloadResMsg), 19);
            messageInfoDic.Add(typeof(FrameWeaponShootReqMsg), 20);
            messageInfoDic.Add(typeof(FrameWeaponShootResMsg), 21);
            messageInfoDic.Add(typeof(WolrdEnterReqMessage), 22);
            messageInfoDic.Add(typeof(WolrdEnterResMessage), 23);
            messageInfoDic.Add(typeof(LoginReqMessage), 24);
            messageInfoDic.Add(typeof(LoginResMessage), 25);
            messageInfoDic.Add(typeof(RegisterAccountReqMessage), 26);
            messageInfoDic.Add(typeof(RegisterAccountResMessage), 27);
            messageInfoDic.Add(typeof(WolrdLeaveReqMessage), 28);
            messageInfoDic.Add(typeof(WolrdLeaveResMessage), 29);
            messageInfoDic.Add(typeof(WorldRoomCreateReqMessage), 30);
            messageInfoDic.Add(typeof(WorldRoomCreateResMessage), 31);

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
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
            messageInfoDic.Add(typeof(FrameBulletSpawnResMsg), 8);
            messageInfoDic.Add(typeof(FrameItemPickReqMsg), 9);
            messageInfoDic.Add(typeof(FrameItemPickResMsg), 10);
            messageInfoDic.Add(typeof(FrameItemSpawnResMsg), 11);
            messageInfoDic.Add(typeof(FrameRollReqMsg), 12);
            messageInfoDic.Add(typeof(FrameRoleMoveReqMsg), 13);
            messageInfoDic.Add(typeof(FrameRoleRotateReqMsg), 14);
            messageInfoDic.Add(typeof(FrameWeaponDropReqMsg), 15);
            messageInfoDic.Add(typeof(FrameWeaponDropResMsg), 16);
            messageInfoDic.Add(typeof(FrameWeaponReloadReqMsg), 17);
            messageInfoDic.Add(typeof(FrameWeaponReloadResMsg), 18);
            messageInfoDic.Add(typeof(FrameWeaponShootReqMsg), 19);
            messageInfoDic.Add(typeof(FrameWeaponShootResMsg), 20);
            messageInfoDic.Add(typeof(WolrdEnterReqMessage), 21);
            messageInfoDic.Add(typeof(WolrdEnterResMessage), 22);
            messageInfoDic.Add(typeof(LoginReqMessage), 23);
            messageInfoDic.Add(typeof(LoginResMessage), 24);
            messageInfoDic.Add(typeof(RegisterAccountReqMessage), 25);
            messageInfoDic.Add(typeof(RegisterAccountResMessage), 26);
            messageInfoDic.Add(typeof(WolrdLeaveReqMessage), 27);
            messageInfoDic.Add(typeof(WolrdLeaveResMessage), 28);
            messageInfoDic.Add(typeof(WorldRoomCreateReqMessage), 29);
            messageInfoDic.Add(typeof(WorldRoomCreateResMessage), 30);

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
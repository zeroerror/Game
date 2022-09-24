using ZeroFrame.Protocol;
using Game.Protocol.Login;
using Game.Protocol.Client2World;
using Game.Protocol.Battle;
using System;using System.Collections.Generic;namespace Game.Protocol
{

    public class ProtocolService
    {
Dictionary<Type, ushort> messageInfoDic;
        public ProtocolService()
        {
            this.messageInfoDic = new Dictionary<Type, ushort>();
            messageInfoDic.Add(typeof(FrameBulletHitRoleResMsg), 0);
            messageInfoDic.Add(typeof(FrameBulletHitWallResMsg), 1);
            messageInfoDic.Add(typeof(FrameBulletSpawnReqMsg), 2);
            messageInfoDic.Add(typeof(FrameBulletSpawnResMsg), 3);
            messageInfoDic.Add(typeof(FrameBulletTearDownResMsg), 4);
            messageInfoDic.Add(typeof(FrameItemPickReqMsg), 5);
            messageInfoDic.Add(typeof(FrameItemPickResMsg), 6);
            messageInfoDic.Add(typeof(FrameItemSpawnResMsg), 7);
            messageInfoDic.Add(typeof(FrameJumpReqMsg), 8);
            messageInfoDic.Add(typeof(FrameOptReqMsg), 9);
            messageInfoDic.Add(typeof(FrameWeaponDropReqMsg), 10);
            messageInfoDic.Add(typeof(FrameWeaponDropResMsg), 11);
            messageInfoDic.Add(typeof(FrameWeaponReloadReqMsg), 12);
            messageInfoDic.Add(typeof(FrameWeaponReloadResMsg), 13);
            messageInfoDic.Add(typeof(FrameWeaponShootReqMsg), 14);
            messageInfoDic.Add(typeof(FrameWeaponShootResMsg), 15);
            messageInfoDic.Add(typeof(FrameWRoleSpawnReqMsg), 16);
            messageInfoDic.Add(typeof(FrameWRoleSpawnResMsg), 17);
            messageInfoDic.Add(typeof(WRoleStateUpdateMsg), 18);
            messageInfoDic.Add(typeof(WolrdEnterReqMessage), 19);
            messageInfoDic.Add(typeof(WolrdEnterResMessage), 20);
            messageInfoDic.Add(typeof(WolrdLeaveReqMessage), 21);
            messageInfoDic.Add(typeof(WolrdLeaveResMessage), 22);
            messageInfoDic.Add(typeof(LoginReqMessage), 23);
            messageInfoDic.Add(typeof(LoginResMessage), 24);
            messageInfoDic.Add(typeof(RegisterAccountReqMessage), 25);
            messageInfoDic.Add(typeof(RegisterAccountResMessage), 26);

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
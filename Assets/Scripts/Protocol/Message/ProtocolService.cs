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
            messageInfoDic.Add(typeof(BattleAirdropSpawnResMsg), 0);
            messageInfoDic.Add(typeof(BattleAirdropTearDownResMsg), 1);
            messageInfoDic.Add(typeof(BattleAssetPointItemsSpawnResMsg), 2);
            messageInfoDic.Add(typeof(BattleBulletLifeTimeOverResMsg), 3);
            messageInfoDic.Add(typeof(BattleRoleSyncMsg), 4);
            messageInfoDic.Add(typeof(BattleStateAndStageReqMsg), 5);
            messageInfoDic.Add(typeof(BattleStateAndStageResMsg), 6);
            messageInfoDic.Add(typeof(FrameBattleRoleSpawnReqMsg), 7);
            messageInfoDic.Add(typeof(FrameBattleRoleSpawnResMsg), 8);
            messageInfoDic.Add(typeof(FrameBulletHitEntityResMsg), 9);
            messageInfoDic.Add(typeof(FrameBulletHitFieldResMsg), 10);
            messageInfoDic.Add(typeof(FrameBulletSpawnResMsg), 11);
            messageInfoDic.Add(typeof(FrameItemPickReqMsg), 12);
            messageInfoDic.Add(typeof(FrameItemPickResMsg), 13);
            messageInfoDic.Add(typeof(FrameRollReqMsg), 14);
            messageInfoDic.Add(typeof(FrameRoleMoveReqMsg), 15);
            messageInfoDic.Add(typeof(FrameRoleRotateReqMsg), 16);
            messageInfoDic.Add(typeof(FrameWeaponDropReqMsg), 17);
            messageInfoDic.Add(typeof(FrameWeaponDropResMsg), 18);
            messageInfoDic.Add(typeof(FrameWeaponShootReqMsg), 19);
            messageInfoDic.Add(typeof(FrameWeaponReloadReqMsg), 20);
            messageInfoDic.Add(typeof(FrameWeaponReloadResMsg), 21);
            messageInfoDic.Add(typeof(FrameWeaponShootResMsg), 22);
            messageInfoDic.Add(typeof(WolrdEnterReqMessage), 23);
            messageInfoDic.Add(typeof(WolrdEnterResMessage), 24);
            messageInfoDic.Add(typeof(LoginReqMessage), 25);
            messageInfoDic.Add(typeof(LoginResMessage), 26);
            messageInfoDic.Add(typeof(RegisterAccountReqMessage), 27);
            messageInfoDic.Add(typeof(RegisterAccountResMessage), 28);
            messageInfoDic.Add(typeof(WolrdLeaveReqMessage), 29);
            messageInfoDic.Add(typeof(WolrdLeaveResMessage), 30);
            messageInfoDic.Add(typeof(WorldRoomCreateReqMessage), 31);
            messageInfoDic.Add(typeof(WorldRoomCreateResMessage), 32);

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
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
            messageInfoDic.Add(typeof(BattleEntitySpawnResMsg), 3);
            messageInfoDic.Add(typeof(BattleEntityTearDownResMsg), 4);
            messageInfoDic.Add(typeof(BattleRoleSyncMsg), 5);
            messageInfoDic.Add(typeof(BattleStateAndStageReqMsg), 6);
            messageInfoDic.Add(typeof(BattleStateAndStageResMsg), 7);
            messageInfoDic.Add(typeof(FrameBattleRoleSpawnReqMsg), 8);
            messageInfoDic.Add(typeof(FrameBattleRoleSpawnResMsg), 9);
            messageInfoDic.Add(typeof(FrameBulletHitEntityResMsg), 10);
            messageInfoDic.Add(typeof(FrameBulletHitFieldResMsg), 11);
            messageInfoDic.Add(typeof(FrameBulletSpawnResMsg), 12);
            messageInfoDic.Add(typeof(FrameItemPickReqMsg), 13);
            messageInfoDic.Add(typeof(FrameItemPickResMsg), 14);
            messageInfoDic.Add(typeof(FrameRollReqMsg), 15);
            messageInfoDic.Add(typeof(FrameRoleMoveReqMsg), 16);
            messageInfoDic.Add(typeof(FrameRoleRotateReqMsg), 17);
            messageInfoDic.Add(typeof(FrameWeaponDropReqMsg), 18);
            messageInfoDic.Add(typeof(FrameWeaponDropResMsg), 19);
            messageInfoDic.Add(typeof(FrameWeaponShootReqMsg), 20);
            messageInfoDic.Add(typeof(FrameWeaponReloadReqMsg), 21);
            messageInfoDic.Add(typeof(FrameWeaponReloadResMsg), 22);
            messageInfoDic.Add(typeof(FrameWeaponShootResMsg), 23);
            messageInfoDic.Add(typeof(WolrdEnterReqMessage), 24);
            messageInfoDic.Add(typeof(WolrdEnterResMessage), 25);
            messageInfoDic.Add(typeof(LoginReqMessage), 26);
            messageInfoDic.Add(typeof(LoginResMessage), 27);
            messageInfoDic.Add(typeof(RegisterAccountReqMessage), 28);
            messageInfoDic.Add(typeof(RegisterAccountResMessage), 29);
            messageInfoDic.Add(typeof(WolrdLeaveReqMessage), 30);
            messageInfoDic.Add(typeof(WolrdLeaveResMessage), 31);
            messageInfoDic.Add(typeof(WorldRoomCreateReqMessage), 32);
            messageInfoDic.Add(typeof(WorldRoomCreateResMessage), 33);

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
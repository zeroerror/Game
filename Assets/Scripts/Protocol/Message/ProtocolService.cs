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
            messageInfoDic.Add(typeof(BattleBulletHitEntityResMsg), 3);
            messageInfoDic.Add(typeof(BattleBulletHitFieldResMsg), 4);
            messageInfoDic.Add(typeof(BattleBulletLifeTimeOverResMsg), 5);
            messageInfoDic.Add(typeof(BattleBulletSpawnResMsg), 6);
            messageInfoDic.Add(typeof(BattleItemPickReqMsg), 7);
            messageInfoDic.Add(typeof(BattleItemPickResMsg), 8);
            messageInfoDic.Add(typeof(BattleRoleMoveReqMsg), 9);
            messageInfoDic.Add(typeof(BattleRoleRollReqMsg), 10);
            messageInfoDic.Add(typeof(BattleRoleRotateReqMsg), 11);
            messageInfoDic.Add(typeof(BattleRoleSpawnReqMsg), 12);
            messageInfoDic.Add(typeof(BattleRoleSpawnResMsg), 13);
            messageInfoDic.Add(typeof(BattleRoleSyncMsg), 14);
            messageInfoDic.Add(typeof(BattleStateAndStageReqMsg), 15);
            messageInfoDic.Add(typeof(BattleStateAndStageResMsg), 16);
            messageInfoDic.Add(typeof(BattleWeaponDropReqMsg), 17);
            messageInfoDic.Add(typeof(BattleWeaponDropResMsg), 18);
            messageInfoDic.Add(typeof(BattleWeaponReloadReqMsg), 19);
            messageInfoDic.Add(typeof(BattleWeaponReloadResMsg), 20);
            messageInfoDic.Add(typeof(BattleWeaponShootReqMsg), 21);
            messageInfoDic.Add(typeof(BattleWeaponShootResMsg), 22);
            messageInfoDic.Add(typeof(WolrdEnterReqMessage), 23);
            messageInfoDic.Add(typeof(WolrdEnterResMessage), 24);
            messageInfoDic.Add(typeof(LoginReqMessage), 25);
            messageInfoDic.Add(typeof(LoginResMessage), 26);
            messageInfoDic.Add(typeof(RegisterAccountReqMessage), 27);
            messageInfoDic.Add(typeof(RegisterAccountResMessage), 28);
            messageInfoDic.Add(typeof(WolrdLeaveReqMsg), 29);
            messageInfoDic.Add(typeof(WolrdLeaveResMsg), 30);
            messageInfoDic.Add(typeof(WorldAllRoomsBacisInfoReqMsg), 31);
            messageInfoDic.Add(typeof(WorldAllRoomsBacisInfoResMsg), 32);
            messageInfoDic.Add(typeof(WorldCreateRoomReqMsg), 33);
            messageInfoDic.Add(typeof(WorldCreateRoomResMsg), 34);
            messageInfoDic.Add(typeof(WorldRoomDismissResMsg), 35);

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
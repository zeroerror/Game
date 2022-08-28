using ZeroFrame.Protocol;
using Game.Protocol.Login;
using Game.Protocol.Client2World;
using Game.Protocol.World;
using System;using System.Collections.Generic;namespace Game.Protocol
{

    public class ProtocolService
    {
Dictionary<Type, ushort> messageInfoDic;
        public ProtocolService()
        {
            this.messageInfoDic = new Dictionary<Type, ushort>();
            messageInfoDic.Add(typeof(WolrdEnterBroadcastResMessage), 0);
            messageInfoDic.Add(typeof(WolrdEnterReqMessage), 1);
            messageInfoDic.Add(typeof(LoginReqMessage), 2);
            messageInfoDic.Add(typeof(LoginResMessage), 3);
            messageInfoDic.Add(typeof(RegisterAccountReqMessage), 4);
            messageInfoDic.Add(typeof(RegisterAccountResMessage), 5);
            messageInfoDic.Add(typeof(FrameBulletHitRoleResMsg), 6);
            messageInfoDic.Add(typeof(FrameBulletSpawnReqMsg), 7);
            messageInfoDic.Add(typeof(FrameBulletSpawnResMsg), 8);
            messageInfoDic.Add(typeof(FrameJumpReqMsg), 9);
            messageInfoDic.Add(typeof(FrameOptReqMsg), 10);
            messageInfoDic.Add(typeof(FrameWRoleSpawnReqMsg), 11);
            messageInfoDic.Add(typeof(FrameWRoleSpawnResMsg), 12);
            messageInfoDic.Add(typeof(WRoleStateUpdateMsg), 13);

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
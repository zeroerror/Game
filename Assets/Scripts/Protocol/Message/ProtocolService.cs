using ZeroFrame.Protocol;
using Game.Protocol.Client2World;
using System;using System.Collections.Generic;namespace Game.Protocol
{

    public class ProtocolService
    {
Dictionary<Type, ushort> messageInfoDic;
        public ProtocolService()
        {
            this.messageInfoDic = new Dictionary<Type, ushort>();
            messageInfoDic.Add(typeof(LoginReqMessage), 0);
            messageInfoDic.Add(typeof(LoginResMessage), 1);
            messageInfoDic.Add(typeof(WolrdEnterBroadcastResMessage), 2);
            messageInfoDic.Add(typeof(WolrdEnterReqMessage), 3);

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
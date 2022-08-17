using System;
using ZeroFrame.Protocol;
using ZeroFrame.Network.TCP;
using Game.Protocol;
using UnityEngine;

namespace Game.Infrastructure.Network.Client
{

    public class NetworkClient : TCPClient
    {

        ProtocolService protocolService;

        public NetworkClient(int maxMessageSize) : base(maxMessageSize)
        {
            protocolService = new ProtocolService();
        }

        public void SendMsg<T>(T msg) where T : IZeroMessage<T>
        {
            // TODO: 添加serviceID和messageID相关协议
            var result = protocolService.GetMessageID<T>();
            base.SendMessage(result.serviceID, result.messageID, msg);
        }

        public void RegistMsg<T>(Action<T> action) where T : IZeroMessage<T>, new()
        {
            var result = protocolService.GetMessageID<T>();
            AddRegister(result.serviceID, result.messageID, () => new T(), action);
        }

        protected void test(){
            
        }

    }

}
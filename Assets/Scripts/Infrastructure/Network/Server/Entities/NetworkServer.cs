using System;
using ZeroFrame.Protocol;
using ZeroFrame.Network.TCP;
using Game.Protocol;

namespace Game.Infrastructure.Network.Server
{

    public class NetworkServer : TCPServer
    {

        ProtocolService protocolService;

        public NetworkServer(int maxMessageSize) : base(maxMessageSize)
        {
            protocolService = new ProtocolService();
        }

        public void SendMsg<T>(int connID, T msg) where T : IZeroMessage<T>
        {
            // TODO: 添加serviceID和messageID相关协议
            var result = protocolService.GetMessageID<T>();
            SendMessage<T>(result.serviceID, result.messageID, msg, connID);
        }

        public void AddRegister<T>(Action<int, T> action) where T : IZeroMessage<T>, new()
        {
            var result = protocolService.GetMessageID<T>();
            base.AddRegister(result.serviceID, result.messageID, () => new T(), action);
        }

    }

}
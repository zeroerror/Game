using System;
using ZeroFrame.Protocol;
using ZeroFrame.Network.TCP;

namespace Game.Infrastructure.Network.Client
{

    public class NetworkClient : TCPClient
    {

        public NetworkClient(int maxMessageSize) : base(maxMessageSize)
        {

        }

        public void SendMsg<T>(byte serviceId, byte messageId, T msg) where T : IZeroMessage<T>
        {
            // TODO: 添加serviceID和messageID相关协议
            base.SendMessage<T>(serviceId, messageId, msg);
        }

        public void RegistMsg<T>(byte serviceId, byte messageId, Action<T> action) where T : IZeroMessage<T>, new()
        {
            AddRegister(serviceId, serviceId, () => new T(), action);
        }

    }

}
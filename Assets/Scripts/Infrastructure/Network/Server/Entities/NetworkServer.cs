using System;
using ZeroFrame.Protocol;
using ZeroFrame.Network.TCP;

namespace Game.Infrastructure.Network.Server
{

    public class NetworkServer : TCPServer
    {

        public NetworkServer(int maxMessageSize) : base(maxMessageSize)
        {

        }

        public void SendMsg<T>(byte serviceId, byte messageId, int connID, T msg) where T : IZeroMessage<T>
        {
            // TODO: 添加serviceID和messageID相关协议
            SendMessage<T>(serviceId, messageId, msg, connID);
        }

        public void RegistMsg<T>(byte serviceId, byte messageId, Action<int, T> action) where T : IZeroMessage<T>, new()
        {
            base.AddRegister(serviceId, serviceId, () => new T(), action);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        new public void AddRegister<T>(byte serviceId, byte messageId, Func<T> generateHandle, Action<int, T> handle) { }


    }

}
using ZeroFrame.Protocol;
using ZeroFrame.Network.TCP;

namespace Game.Network
{

    public class NetworkClient : TCPClient
    {

        public NetworkClient(int maxMessageSize) : base(maxMessageSize)
        {

        }

        public void SendMsg<T>(T msg) where T : IZeroMessage<T>
        {
            // TODO: 添加serviceID和messageID相关协议
            byte serviceId = 1;
            byte messageId = 1;
            SendMsg<T>(serviceId, messageId, msg);
        }

    }

}
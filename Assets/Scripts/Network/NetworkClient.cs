using ZeroFrame.Network.TCP;

namespace Game.Network
{

    public class NetworkClient : TCPClient
    {

        public NetworkClient(int maxMessageSize) : base(maxMessageSize)
        {

        }
        
    }

}
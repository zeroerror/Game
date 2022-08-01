using ZeroFrame.Network.TCP;

namespace Game.Network
{

    public class NetworkServer : TCPServer
    {

        public NetworkServer(int maxMessageSize) : base(maxMessageSize)
        {

        }
        
    }

}
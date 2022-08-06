using ZeroFrame.Protocol;
using ZeroFrame.Buffer;

namespace Game.Protocol.Client2World
{

    public class LoginResMessage : IZeroMessage<LoginResMessage>
    {

        public byte status;
        public string userToken;


        public void FromBytes(byte[] src, ref int offset)
        {
            status = BufferReader.ReadByte(src, ref offset);
            userToken = BufferReader.ReadUTF8String(src, ref offset);
        }

        public byte[] ToBytes()
        {
            byte[] src = new byte[1000];
            int offset = 0;
            BufferWriter.WriteByte(src, status, ref offset);
            BufferWriter.WriteUTF8String(src, userToken, ref offset);
            return src;
        }
    }

}
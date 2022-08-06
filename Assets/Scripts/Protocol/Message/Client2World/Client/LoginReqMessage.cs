using ZeroFrame.Protocol;
using ZeroFrame.Buffer;

namespace Game.Protocol.Client2World
{

    public class LoginReqMessage : IZeroMessage<LoginReqMessage>
    {

        public string account;
        public string pwd;


        public void FromBytes(byte[] src, ref int offset)
        {
            account = BufferReader.ReadUTF8String(src, ref offset);
            pwd = BufferReader.ReadUTF8String(src, ref offset);
        }

        public byte[] ToBytes()
        {
            byte[] src = new byte[1000];
            int offset = 0;
            BufferWriter.WriteUTF8String(src, account, ref offset);
            BufferWriter.WriteUTF8String(src, pwd, ref offset);
            return src;
        }
    }

}
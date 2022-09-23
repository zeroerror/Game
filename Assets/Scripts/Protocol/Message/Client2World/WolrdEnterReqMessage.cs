using ZeroFrame.Protocol;
using ZeroFrame.Buffer;
namespace Game.Protocol.Client2World
{

    [ZeroMessage]
    public class WolrdEnterReqMessage : IZeroMessage<WolrdEnterReqMessage>
    {

        public string account;

        public void FromBytes(byte[] src, ref int offset)
        {
            account = BufferReader.ReadUTF8String(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUTF8String(result, account, ref offset);
            return result;
        }

    }

}
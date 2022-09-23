using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Client2World
{

    [ZeroMessage]
    public class WolrdEnterReqMessage :IZeroMessage<WolrdEnterReqMessage>{

        public void FromBytes(byte[] src, ref int offset)
        {
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            return result;
        }

    }

}
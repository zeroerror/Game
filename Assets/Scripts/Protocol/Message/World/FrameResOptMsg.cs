using ZeroFrame.Protocol;
using System;
using ZeroFrame.Buffer;
namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameResOptMsg
:IZeroMessage<FrameResOptMsg>    {
        public int serverFrameIndex;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrameIndex = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrameIndex, ref offset);
            return result;
        }

    }

}
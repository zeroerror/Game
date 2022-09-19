using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameJumpReqMsg :IZeroMessage<FrameJumpReqMsg>{
        public byte wRid;

        public void FromBytes(byte[] src, ref int offset)
        {
            wRid = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, wRid, ref offset);
            return result;
        }

    }

}
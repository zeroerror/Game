using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameJumpReqMsg :IZeroMessage<FrameJumpReqMsg>{
        public int clientFrameIndex;
        public byte wRid;

        public void FromBytes(byte[] src, ref int offset)
        {
            clientFrameIndex = BufferReader.ReadInt32(src, ref offset);
            wRid = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, clientFrameIndex, ref offset);
            BufferWriter.WriteByte(result, wRid, ref offset);
            return result;
        }

    }

}
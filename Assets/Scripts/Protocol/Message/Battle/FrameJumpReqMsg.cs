using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameJumpReqMsg :IZeroMessage<FrameJumpReqMsg>{
        public byte entityId;

        public void FromBytes(byte[] src, ref int offset)
        {
            entityId = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, entityId, ref offset);
            return result;
        }

    }

}
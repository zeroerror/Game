using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameRollReqMsg :IZeroMessage<FrameRollReqMsg>{
        public byte entityId;
        public int dirX;
        public int dirY;
        public int dirZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            entityId = BufferReader.ReadByte(src, ref offset);
            dirX = BufferReader.ReadInt32(src, ref offset);
            dirY = BufferReader.ReadInt32(src, ref offset);
            dirZ = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, entityId, ref offset);
            BufferWriter.WriteInt32(result, dirX, ref offset);
            BufferWriter.WriteInt32(result, dirY, ref offset);
            BufferWriter.WriteInt32(result, dirZ, ref offset);
            return result;
        }

    }

}
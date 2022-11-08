using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameBulletHitEntityResMsg :IZeroMessage<FrameBulletHitEntityResMsg>{
        public int serverFrame;
        public ushort bulletEntityID;
        public byte entityType;
        public byte entityID;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            bulletEntityID = BufferReader.ReadUInt16(src, ref offset);
            entityType = BufferReader.ReadByte(src, ref offset);
            entityID = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteUInt16(result, bulletEntityID, ref offset);
            BufferWriter.WriteByte(result, entityType, ref offset);
            BufferWriter.WriteByte(result, entityID, ref offset);
            return result;
        }
    }

}
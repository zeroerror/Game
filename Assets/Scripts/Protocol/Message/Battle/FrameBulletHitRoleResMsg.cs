using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameBulletHitRoleResMsg :IZeroMessage<FrameBulletHitRoleResMsg>{
        public int serverFrame;
        public ushort bulletId;
        public byte entityId;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            bulletId = BufferReader.ReadUInt16(src, ref offset);
            entityId = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteUInt16(result, bulletId, ref offset);
            BufferWriter.WriteByte(result, entityId, ref offset);
            return result;
        }
    }

}
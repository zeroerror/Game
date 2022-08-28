using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameBulletSpawnResMsg :IZeroMessage<FrameBulletSpawnResMsg>{
        public int serverFrameIndex;
        public byte wRid;
        public ushort bulletId;
        public short shootDirX;   // (8 8) 整数部8位 sbyte -128 --- +127） 小数部分16位 byte(0 --- +255) 0.00到0.99
        public short shootDirY;
        public short shootDirZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrameIndex = BufferReader.ReadInt32(src, ref offset);
            wRid = BufferReader.ReadByte(src, ref offset);
            bulletId = BufferReader.ReadUInt16(src, ref offset);
            shootDirX = BufferReader.ReadInt16(src, ref offset);
            shootDirY = BufferReader.ReadInt16(src, ref offset);
            shootDirZ = BufferReader.ReadInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrameIndex, ref offset);
            BufferWriter.WriteByte(result, wRid, ref offset);
            BufferWriter.WriteUInt16(result, bulletId, ref offset);
            BufferWriter.WriteInt16(result, shootDirX, ref offset);
            BufferWriter.WriteInt16(result, shootDirY, ref offset);
            BufferWriter.WriteInt16(result, shootDirZ, ref offset);
            return result;
        }
    }

}
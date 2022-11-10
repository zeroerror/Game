using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleBulletSpawnResMsg :IZeroMessage<BattleBulletSpawnResMsg>{
        public int serverFrame;
        public byte bulletType;
        public byte weaponID;
        public ushort bulletID;
        public int startPosX;   
        public int startPosY;
        public int startPosZ;
        public short fireDirX;   // (8 8) 整数部8位 sbyte -128 --- +127） 小数部分16位 byte(0 --- +255) 0.00到0.99
        public short fireDirZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            bulletType = BufferReader.ReadByte(src, ref offset);
            weaponID = BufferReader.ReadByte(src, ref offset);
            bulletID = BufferReader.ReadUInt16(src, ref offset);
            startPosX = BufferReader.ReadInt32(src, ref offset);
            startPosY = BufferReader.ReadInt32(src, ref offset);
            startPosZ = BufferReader.ReadInt32(src, ref offset);
            fireDirX = BufferReader.ReadInt16(src, ref offset);
            fireDirZ = BufferReader.ReadInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByte(result, bulletType, ref offset);
            BufferWriter.WriteByte(result, weaponID, ref offset);
            BufferWriter.WriteUInt16(result, bulletID, ref offset);
            BufferWriter.WriteInt32(result, startPosX, ref offset);
            BufferWriter.WriteInt32(result, startPosY, ref offset);
            BufferWriter.WriteInt32(result, startPosZ, ref offset);
            BufferWriter.WriteInt16(result, fireDirX, ref offset);
            BufferWriter.WriteInt16(result, fireDirZ, ref offset);
            return result;
        }
    }

}
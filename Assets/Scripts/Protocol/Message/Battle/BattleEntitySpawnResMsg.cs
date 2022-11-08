using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleEntitySpawnResMsg :IZeroMessage<BattleEntitySpawnResMsg>{
        public int serverFrame;
        public byte entityType;
        public byte subType;
        public int entityID;
        public int posX;  // (16 16) 整数部16位 short -32768 --- +32767 小数部分16位 ushort(0 --- +65535) 0.0000到0.9999
        public int posY;
        public int posZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            entityType = BufferReader.ReadByte(src, ref offset);
            subType = BufferReader.ReadByte(src, ref offset);
            entityID = BufferReader.ReadInt32(src, ref offset);
            posX = BufferReader.ReadInt32(src, ref offset);
            posY = BufferReader.ReadInt32(src, ref offset);
            posZ = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByte(result, entityType, ref offset);
            BufferWriter.WriteByte(result, subType, ref offset);
            BufferWriter.WriteInt32(result, entityID, ref offset);
            BufferWriter.WriteInt32(result, posX, ref offset);
            BufferWriter.WriteInt32(result, posY, ref offset);
            BufferWriter.WriteInt32(result, posZ, ref offset);
            return result;
        }
    }

}
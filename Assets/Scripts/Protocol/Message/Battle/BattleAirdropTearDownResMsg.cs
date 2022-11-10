using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleAirdropTearDownResMsg :IZeroMessage<BattleAirdropTearDownResMsg>{

        public int airdropID;
        public byte spawnEntityType;
        public byte spawnSubType;
        public int spawnEntityID;
        public int spawnPosX;
        public int spawnPosY;
        public int spawnPosZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            airdropID = BufferReader.ReadInt32(src, ref offset);
            spawnEntityType = BufferReader.ReadByte(src, ref offset);
            spawnSubType = BufferReader.ReadByte(src, ref offset);
            spawnEntityID = BufferReader.ReadInt32(src, ref offset);
            spawnPosX = BufferReader.ReadInt32(src, ref offset);
            spawnPosY = BufferReader.ReadInt32(src, ref offset);
            spawnPosZ = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, airdropID, ref offset);
            BufferWriter.WriteByte(result, spawnEntityType, ref offset);
            BufferWriter.WriteByte(result, spawnSubType, ref offset);
            BufferWriter.WriteInt32(result, spawnEntityID, ref offset);
            BufferWriter.WriteInt32(result, spawnPosX, ref offset);
            BufferWriter.WriteInt32(result, spawnPosY, ref offset);
            BufferWriter.WriteInt32(result, spawnPosZ, ref offset);
            return result;
        }
    }

}
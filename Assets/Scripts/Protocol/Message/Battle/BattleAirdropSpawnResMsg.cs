using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleAirdropSpawnResMsg :IZeroMessage<BattleAirdropSpawnResMsg>{

        public byte airdropEntityType;
        public byte subType;
        public int entityID;
        public int posX;
        public int posY;
        public int posZ;
        public int battleStage;

        public void FromBytes(byte[] src, ref int offset)
        {
            airdropEntityType = BufferReader.ReadByte(src, ref offset);
            subType = BufferReader.ReadByte(src, ref offset);
            entityID = BufferReader.ReadInt32(src, ref offset);
            posX = BufferReader.ReadInt32(src, ref offset);
            posY = BufferReader.ReadInt32(src, ref offset);
            posZ = BufferReader.ReadInt32(src, ref offset);
            battleStage = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, airdropEntityType, ref offset);
            BufferWriter.WriteByte(result, subType, ref offset);
            BufferWriter.WriteInt32(result, entityID, ref offset);
            BufferWriter.WriteInt32(result, posX, ref offset);
            BufferWriter.WriteInt32(result, posY, ref offset);
            BufferWriter.WriteInt32(result, posZ, ref offset);
            BufferWriter.WriteInt32(result, battleStage, ref offset);
            return result;
        }
    }

}
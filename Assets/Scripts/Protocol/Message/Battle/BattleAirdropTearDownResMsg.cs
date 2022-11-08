using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleAirdropTearDownResMsg :IZeroMessage<BattleAirdropTearDownResMsg>{

        public int entityID;
        public byte subType;

        public void FromBytes(byte[] src, ref int offset)
        {
            entityID = BufferReader.ReadInt32(src, ref offset);
            subType = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, entityID, ref offset);
            BufferWriter.WriteByte(result, subType, ref offset);
            return result;
        }
    }

}
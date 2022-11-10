using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleItemPickResMsg
:IZeroMessage<BattleItemPickResMsg>{
        public int serverFrame;
        public byte roleID;
        public byte entityType;
        public ushort itemID;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            roleID = BufferReader.ReadByte(src, ref offset);
            entityType = BufferReader.ReadByte(src, ref offset);
            itemID = BufferReader.ReadUInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByte(result, roleID, ref offset);
            BufferWriter.WriteByte(result, entityType, ref offset);
            BufferWriter.WriteUInt16(result, itemID, ref offset);
            return result;
        }

        
    }

}
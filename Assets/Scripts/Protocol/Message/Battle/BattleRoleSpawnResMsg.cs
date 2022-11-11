using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleRoleSpawnResMsg :IZeroMessage<BattleRoleSpawnResMsg>{
        public int serverFrame;
        public byte entityId;
        public byte controlType;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            entityId = BufferReader.ReadByte(src, ref offset);
            controlType = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByte(result, entityId, ref offset);
            BufferWriter.WriteByte(result, controlType, ref offset);
            return result;
        }
    }

}
using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameBattleRoleSpawnReqMsg :IZeroMessage<FrameBattleRoleSpawnReqMsg>{

        public int typeID;
        public byte controlType;

        public void FromBytes(byte[] src, ref int offset)
        {
            typeID = BufferReader.ReadInt32(src, ref offset);
            controlType = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, typeID, ref offset);
            BufferWriter.WriteByte(result, controlType, ref offset);
            return result;
        }
    }

}
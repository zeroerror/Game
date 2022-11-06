using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameBattleRoleSpawnReqMsg :IZeroMessage<FrameBattleRoleSpawnReqMsg>{

        public byte controlType;

        public void FromBytes(byte[] src, ref int offset)
        {
            controlType = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, controlType, ref offset);
            return result;
        }
    }

}
using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleStateAndStageResMsg :IZeroMessage<BattleStateAndStageResMsg>{

        public byte state;
        public int curMaintainFrame;
        public int stage;

        public void FromBytes(byte[] src, ref int offset)
        {
            state = BufferReader.ReadByte(src, ref offset);
            curMaintainFrame = BufferReader.ReadInt32(src, ref offset);
            stage = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, state, ref offset);
            BufferWriter.WriteInt32(result, curMaintainFrame, ref offset);
            BufferWriter.WriteInt32(result, stage, ref offset);
            return result;
        }
    }

}
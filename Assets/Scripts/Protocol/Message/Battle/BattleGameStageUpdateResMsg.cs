using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleGameStageUpdateResMsg :IZeroMessage<BattleGameStageUpdateResMsg>{

       public int gameStage;

        public void FromBytes(byte[] src, ref int offset)
        {
            gameStage = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, gameStage, ref offset);
            return result;
        }
    }

}
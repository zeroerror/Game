using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleGameStateEnterResMsg :IZeroMessage<BattleGameStateEnterResMsg>{

       public int gameState;
       public int gameStage;

        public void FromBytes(byte[] src, ref int offset)
        {
            gameState = BufferReader.ReadInt32(src, ref offset);
            gameStage = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, gameState, ref offset);
            BufferWriter.WriteInt32(result, gameStage, ref offset);
            return result;
        }
    }

}
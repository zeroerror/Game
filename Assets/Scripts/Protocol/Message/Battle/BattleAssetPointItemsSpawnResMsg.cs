using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleAssetPointItemsSpawnResMsg
:IZeroMessage<BattleAssetPointItemsSpawnResMsg>{
        public int serverFrame;
        public byte[] entityTypeArray;    // 武器、子弹
        public byte[] subtypeArray;  //武器：手枪、步枪、榴弹枪  子弹：。。。。
        public int[] entityIDArray;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            entityTypeArray = BufferReader.ReadByteArray(src, ref offset);
            subtypeArray = BufferReader.ReadByteArray(src, ref offset);
            entityIDArray = BufferReader.ReadInt32Array(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByteArray(result, entityTypeArray, ref offset);
            BufferWriter.WriteByteArray(result, subtypeArray, ref offset);
            BufferWriter.WriteInt32Array(result, entityIDArray, ref offset);
            return result;
        }
    }

}
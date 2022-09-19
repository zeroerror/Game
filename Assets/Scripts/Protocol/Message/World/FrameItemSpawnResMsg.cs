using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameItemSpawnResMsg
:IZeroMessage<FrameItemSpawnResMsg>{
        public int serverFrame;
        public byte[] itemTypeArray;    // 武器、子弹
        public byte[] subtypeArray;  //武器：手枪、步枪、榴弹枪  子弹：。。。。
        public ushort[] entityIdArray;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            itemTypeArray = BufferReader.ReadByteArray(src, ref offset);
            subtypeArray = BufferReader.ReadByteArray(src, ref offset);
            entityIdArray = BufferReader.ReadUInt16Array(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByteArray(result, itemTypeArray, ref offset);
            BufferWriter.WriteByteArray(result, subtypeArray, ref offset);
            BufferWriter.WriteUInt16Array(result, entityIdArray, ref offset);
            return result;
        }
    }

}
using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameItemPickResMsg
:IZeroMessage<FrameItemPickResMsg>{
        public int serverFrame;
        public byte masterEntityID;
        public byte itemType;
        public ushort itemEntityID;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            masterEntityID = BufferReader.ReadByte(src, ref offset);
            itemType = BufferReader.ReadByte(src, ref offset);
            itemEntityID = BufferReader.ReadUInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByte(result, masterEntityID, ref offset);
            BufferWriter.WriteByte(result, itemType, ref offset);
            BufferWriter.WriteUInt16(result, itemEntityID, ref offset);
            return result;
        }

        
    }

}
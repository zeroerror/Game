using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameItemPickResMsg
:IZeroMessage<FrameItemPickResMsg>{
        public int serverFrame;
        public byte wRid;
        public byte itemType;
        public ushort entityId;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            wRid = BufferReader.ReadByte(src, ref offset);
            itemType = BufferReader.ReadByte(src, ref offset);
            entityId = BufferReader.ReadUInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByte(result, wRid, ref offset);
            BufferWriter.WriteByte(result, itemType, ref offset);
            BufferWriter.WriteUInt16(result, entityId, ref offset);
            return result;
        }

        
    }

}
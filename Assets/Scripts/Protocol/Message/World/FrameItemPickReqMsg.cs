using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameItemPickReqMsg
:IZeroMessage<FrameItemPickReqMsg>{
        public byte wRid;
        public byte itemType;
        public ushort entityId;

        public void FromBytes(byte[] src, ref int offset)
        {
            wRid = BufferReader.ReadByte(src, ref offset);
            itemType = BufferReader.ReadByte(src, ref offset);
            entityId = BufferReader.ReadUInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, wRid, ref offset);
            BufferWriter.WriteByte(result, itemType, ref offset);
            BufferWriter.WriteUInt16(result, entityId, ref offset);
            return result;
        }
        
    }

}
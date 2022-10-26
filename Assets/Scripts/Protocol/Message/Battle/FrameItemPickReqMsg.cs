using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameItemPickReqMsg
:IZeroMessage<FrameItemPickReqMsg>{
        public byte entityID;
        public byte entityType;
        public ushort entityId;

        public void FromBytes(byte[] src, ref int offset)
        {
            entityID = BufferReader.ReadByte(src, ref offset);
            entityType = BufferReader.ReadByte(src, ref offset);
            entityId = BufferReader.ReadUInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, entityID, ref offset);
            BufferWriter.WriteByte(result, entityType, ref offset);
            BufferWriter.WriteUInt16(result, entityId, ref offset);
            return result;
        }
        
    }

}
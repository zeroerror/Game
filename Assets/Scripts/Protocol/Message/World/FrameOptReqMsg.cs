using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameOptReqMsg:IZeroMessage<FrameOptReqMsg>{
        public sbyte optTypeId;
        public ulong msg;

        public void FromBytes(byte[] src, ref int offset)
        {
            optTypeId = BufferReader.ReadSByte(src, ref offset);
            msg = BufferReader.ReadUInt64(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteSByte(result, optTypeId, ref offset);
            BufferWriter.WriteUInt64(result, msg, ref offset);
            return result;
        }

    }

}
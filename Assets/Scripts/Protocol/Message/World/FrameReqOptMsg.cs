using ZeroFrame.Protocol;
using System;
using ZeroFrame.Buffer;
namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameReqOptMsg
:IZeroMessage<FrameReqOptMsg>    {
        public int clientFrameIndex;
        public sbyte optTypeId;
        public int msg;

        public void FromBytes(byte[] src, ref int offset)
        {
            clientFrameIndex = BufferReader.ReadInt32(src, ref offset);
            optTypeId = BufferReader.ReadSByte(src, ref offset);
            msg = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, clientFrameIndex, ref offset);
            BufferWriter.WriteSByte(result, optTypeId, ref offset);
            BufferWriter.WriteInt32(result, msg, ref offset);
            return result;
        }

    }

}
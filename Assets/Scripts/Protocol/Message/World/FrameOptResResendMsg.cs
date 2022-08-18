using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    /// <summary>
    /// 补发Opt数据 TODO:完善ZeroBuffer改为用数组一次性传输
    /// </summary>
    [ZeroMessage]
    public class FrameOptResResendMsg:IZeroMessage<FrameOptResResendMsg>{
       public int serverFrameIndex;
        public sbyte optTypeId;
        public int msg;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrameIndex = BufferReader.ReadInt32(src, ref offset);
            optTypeId = BufferReader.ReadSByte(src, ref offset);
            msg = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrameIndex, ref offset);
            BufferWriter.WriteSByte(result, optTypeId, ref offset);
            BufferWriter.WriteInt32(result, msg, ref offset);
            return result;
        }
    }


}
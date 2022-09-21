using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameWRoleSpawnResMsg :IZeroMessage<FrameWRoleSpawnResMsg>{
        public int serverFrame;
        public byte wRoleId;
        public bool isOwner;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            wRoleId = BufferReader.ReadByte(src, ref offset);
            isOwner = BufferReader.ReadBool(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByte(result, wRoleId, ref offset);
            BufferWriter.WriteBool(result, isOwner, ref offset);
            return result;
        }
    }

}
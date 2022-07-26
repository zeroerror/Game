using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameWRoleSpawnReqMsg:IZeroMessage<FrameWRoleSpawnReqMsg>{
        public int clientFrameIndex;

        public void FromBytes(byte[] src, ref int offset)
        {
            clientFrameIndex = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, clientFrameIndex, ref offset);
            return result;
        }
    }

}
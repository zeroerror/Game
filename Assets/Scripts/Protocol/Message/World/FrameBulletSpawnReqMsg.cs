using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameBulletSpawnReqMsg :IZeroMessage<FrameBulletSpawnReqMsg>{
        public int clientFrameIndex;
        public byte bulletType;
        public byte wRid;
        public int targetPosX;   // (16 16) 整数部16位 short -32768 --- +32767 小数部分16位 ushort(0 --- +65535) 0.0000到0.9999
        public int targetPosY;
        public int targetPosZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            clientFrameIndex = BufferReader.ReadInt32(src, ref offset);
            bulletType = BufferReader.ReadByte(src, ref offset);
            wRid = BufferReader.ReadByte(src, ref offset);
            targetPosX = BufferReader.ReadInt32(src, ref offset);
            targetPosY = BufferReader.ReadInt32(src, ref offset);
            targetPosZ = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, clientFrameIndex, ref offset);
            BufferWriter.WriteByte(result, bulletType, ref offset);
            BufferWriter.WriteByte(result, wRid, ref offset);
            BufferWriter.WriteInt32(result, targetPosX, ref offset);
            BufferWriter.WriteInt32(result, targetPosY, ref offset);
            BufferWriter.WriteInt32(result, targetPosZ, ref offset);
            return result;
        }
    }

}
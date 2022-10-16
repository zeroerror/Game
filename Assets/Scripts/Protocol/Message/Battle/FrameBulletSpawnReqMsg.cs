using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameBulletSpawnReqMsg :IZeroMessage<FrameBulletSpawnReqMsg>{
        public byte bulletType;
        public byte masterEntityId;
        public int targetPosX;   // (16 16) 整数部16位 short -32768 --- +32767 小数部分16位 ushort(0 --- +65535) 0.0000到0.9999
        public int targetPosY;
        public int targetPosZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            bulletType = BufferReader.ReadByte(src, ref offset);
            masterEntityId = BufferReader.ReadByte(src, ref offset);
            targetPosX = BufferReader.ReadInt32(src, ref offset);
            targetPosY = BufferReader.ReadInt32(src, ref offset);
            targetPosZ = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, bulletType, ref offset);
            BufferWriter.WriteByte(result, masterEntityId, ref offset);
            BufferWriter.WriteInt32(result, targetPosX, ref offset);
            BufferWriter.WriteInt32(result, targetPosY, ref offset);
            BufferWriter.WriteInt32(result, targetPosZ, ref offset);
            return result;
        }
    }

}
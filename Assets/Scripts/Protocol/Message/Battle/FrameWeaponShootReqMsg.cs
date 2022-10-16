using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameWeaponShootReqMsg :IZeroMessage<FrameWeaponShootReqMsg>{
        public byte masterId;
        public int startPosX;   // (16 16) 整数部16位 short -32768 --- +32767 小数部分16位 ushort(0 --- +65535) 0.0000到0.9999
        public int startPosY;
        public int startPosZ;
        public int endPosX;   // (16 16) 整数部16位 short -32768 --- +32767 小数部分16位 ushort(0 --- +65535) 0.0000到0.9999
        public int endPosY;
        public int endPosZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            masterId = BufferReader.ReadByte(src, ref offset);
            startPosX = BufferReader.ReadInt32(src, ref offset);
            startPosY = BufferReader.ReadInt32(src, ref offset);
            startPosZ = BufferReader.ReadInt32(src, ref offset);
            endPosX = BufferReader.ReadInt32(src, ref offset);
            endPosY = BufferReader.ReadInt32(src, ref offset);
            endPosZ = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, masterId, ref offset);
            BufferWriter.WriteInt32(result, startPosX, ref offset);
            BufferWriter.WriteInt32(result, startPosY, ref offset);
            BufferWriter.WriteInt32(result, startPosZ, ref offset);
            BufferWriter.WriteInt32(result, endPosX, ref offset);
            BufferWriter.WriteInt32(result, endPosY, ref offset);
            BufferWriter.WriteInt32(result, endPosZ, ref offset);
            return result;
        }

    }

}
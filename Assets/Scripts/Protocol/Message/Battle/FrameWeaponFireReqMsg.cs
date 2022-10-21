using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameWeaponFireReqMsg :IZeroMessage<FrameWeaponFireReqMsg>{
        public byte masterId;
        public int firePointPosX;
        public int firePointPosY;
        public int firePointPosZ;
        public short dirX;
        public short dirZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            masterId = BufferReader.ReadByte(src, ref offset);
            firePointPosX = BufferReader.ReadInt32(src, ref offset);
            firePointPosY = BufferReader.ReadInt32(src, ref offset);
            firePointPosZ = BufferReader.ReadInt32(src, ref offset);
            dirX = BufferReader.ReadInt16(src, ref offset);
            dirZ = BufferReader.ReadInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, masterId, ref offset);
            BufferWriter.WriteInt32(result, firePointPosX, ref offset);
            BufferWriter.WriteInt32(result, firePointPosY, ref offset);
            BufferWriter.WriteInt32(result, firePointPosZ, ref offset);
            BufferWriter.WriteInt16(result, dirX, ref offset);
            BufferWriter.WriteInt16(result, dirZ, ref offset);
            return result;
        }

    }

}
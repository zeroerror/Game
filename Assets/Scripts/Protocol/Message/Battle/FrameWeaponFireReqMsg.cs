using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameWeaponShootReqMsg :IZeroMessage<FrameWeaponShootReqMsg>{
        public byte weaponID;
        public int firePointPosX;
        public int firePointPosY;
        public int firePointPosZ;
        public short fireDirX;
        public short fireDirZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            weaponID = BufferReader.ReadByte(src, ref offset);
            firePointPosX = BufferReader.ReadInt32(src, ref offset);
            firePointPosY = BufferReader.ReadInt32(src, ref offset);
            firePointPosZ = BufferReader.ReadInt32(src, ref offset);
            fireDirX = BufferReader.ReadInt16(src, ref offset);
            fireDirZ = BufferReader.ReadInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, weaponID, ref offset);
            BufferWriter.WriteInt32(result, firePointPosX, ref offset);
            BufferWriter.WriteInt32(result, firePointPosY, ref offset);
            BufferWriter.WriteInt32(result, firePointPosZ, ref offset);
            BufferWriter.WriteInt16(result, fireDirX, ref offset);
            BufferWriter.WriteInt16(result, fireDirZ, ref offset);
            return result;
        }

    }

}
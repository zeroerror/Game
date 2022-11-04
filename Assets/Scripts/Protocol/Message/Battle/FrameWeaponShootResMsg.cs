using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameWeaponShootResMsg :IZeroMessage<FrameWeaponShootResMsg>{
        public byte weaponID;

        public void FromBytes(byte[] src, ref int offset)
        {
            weaponID = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, weaponID, ref offset);
            return result;
        }
    }

}
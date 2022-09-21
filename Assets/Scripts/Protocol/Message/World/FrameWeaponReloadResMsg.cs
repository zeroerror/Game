using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameWeaponReloadResMsg:IZeroMessage<FrameWeaponReloadResMsg>{
        public byte masterId;
        public byte reloadBulletNum;

        public void FromBytes(byte[] src, ref int offset)
        {
            masterId = BufferReader.ReadByte(src, ref offset);
            reloadBulletNum = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, masterId, ref offset);
            BufferWriter.WriteByte(result, reloadBulletNum, ref offset);
            return result;
        }
    }

}
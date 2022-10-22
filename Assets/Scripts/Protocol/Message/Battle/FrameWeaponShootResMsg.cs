using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameWeaponShootResMsg :IZeroMessage<FrameWeaponShootResMsg>{
        public byte masterId;

        public void FromBytes(byte[] src, ref int offset)
        {
            masterId = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, masterId, ref offset);
            return result;
        }
    }

}
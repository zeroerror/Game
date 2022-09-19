using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameWeaponReloadResMsg:IZeroMessage<FrameWeaponReloadResMsg>{
        public ushort entityId; //装弹武器entityId

        public void FromBytes(byte[] src, ref int offset)
        {
            entityId = BufferReader.ReadUInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUInt16(result, entityId, ref offset);
            return result;
        }
    }

}
using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameWeaponDropResMsg
:IZeroMessage<FrameWeaponDropResMsg>{
        public ushort weaponID; //被丢弃武器entityId
        public byte masterID;

        public void FromBytes(byte[] src, ref int offset)
        {
            weaponID = BufferReader.ReadUInt16(src, ref offset);
            masterID = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUInt16(result, weaponID, ref offset);
            BufferWriter.WriteByte(result, masterID, ref offset);
            return result;
        }

    }

}
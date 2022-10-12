using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameWeaponDropReqMsg
:IZeroMessage<FrameWeaponDropReqMsg>{
        public ushort entityId; //被丢弃武器entityId
        public int masterId;

        public void FromBytes(byte[] src, ref int offset)
        {
            entityId = BufferReader.ReadUInt16(src, ref offset);
            masterId = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUInt16(result, entityId, ref offset);
            BufferWriter.WriteInt32(result, masterId, ref offset);
            return result;
        }


    }

}
using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleWeaponDropReqMsg
:IZeroMessage<BattleWeaponDropReqMsg>{
        public ushort entityID; //被丢弃武器entityId
        public int masterId;

        public void FromBytes(byte[] src, ref int offset)
        {
            entityID = BufferReader.ReadUInt16(src, ref offset);
            masterId = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUInt16(result, entityID, ref offset);
            BufferWriter.WriteInt32(result, masterId, ref offset);
            return result;
        }


    }

}
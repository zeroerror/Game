using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class FrameWeaponReloadReqMsg
:IZeroMessage<FrameWeaponReloadReqMsg>{
        public int masterId;

        public void FromBytes(byte[] src, ref int offset)
        {
            masterId = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, masterId, ref offset);
            return result;
        }

    }

}
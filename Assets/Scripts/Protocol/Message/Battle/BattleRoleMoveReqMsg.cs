using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleRoleMoveReqMsg:IZeroMessage<BattleRoleMoveReqMsg>{
        public ulong msg;

        public void FromBytes(byte[] src, ref int offset)
        {
            msg = BufferReader.ReadUInt64(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUInt64(result, msg, ref offset);
            return result;
        }

    }

}
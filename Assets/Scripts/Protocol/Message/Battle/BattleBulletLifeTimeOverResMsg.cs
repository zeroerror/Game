using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleBulletLifeTimeOverResMsg :IZeroMessage<BattleBulletLifeTimeOverResMsg>{

        public int entityID;

        public void FromBytes(byte[] src, ref int offset)
        {
            entityID = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, entityID, ref offset);
            return result;
        }
    }

}
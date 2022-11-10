using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleBulletHitFieldResMsg :IZeroMessage<BattleBulletHitFieldResMsg>{
        public int serverFrame;
        public ushort bulletEntityID;
        public int posX; //(8  8)  (8  8)   整数部16位 int16（-32768 --- +32767） 小数部分16位 uint16(0 --- +65535) 0.0000到0.9999
        public int posY; //(8  8)  (8  8)   整数部16位 int16（-32768 --- +32767） 小数部分16位 uint16(0 --- +65535) 0.0000到0.9999
        public int posZ; //(8  8)  (8  8)   整数部16位 int16（-32768 --- +32767） 小数部分16位 uint16(0 --- +65535) 0.0000到0.9999

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            bulletEntityID = BufferReader.ReadUInt16(src, ref offset);
            posX = BufferReader.ReadInt32(src, ref offset);
            posY = BufferReader.ReadInt32(src, ref offset);
            posZ = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteUInt16(result, bulletEntityID, ref offset);
            BufferWriter.WriteInt32(result, posX, ref offset);
            BufferWriter.WriteInt32(result, posY, ref offset);
            BufferWriter.WriteInt32(result, posZ, ref offset);
            return result;
        }
    }

}
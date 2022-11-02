using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Battle
{

    [ZeroMessage]
    public class BattleRoleSyncMsg :IZeroMessage<BattleRoleSyncMsg>{
        public int serverFrame;
        public byte entityId;
        public int roleState;
        public int posX; //(8  8)  (8  8)   整数部16位 int16（-32768 --- +32767） 小数部分16位 uint16(0 --- +65535) 0.0000到0.9999
        public int posY;
        public int posZ;
        public int eulerX;
        public int eulerY;
        public int eulerZ;
        public int velocityX;
        public int velocityY;
        public int velocityZ;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            entityId = BufferReader.ReadByte(src, ref offset);
            roleState = BufferReader.ReadInt32(src, ref offset);
            posX = BufferReader.ReadInt32(src, ref offset);
            posY = BufferReader.ReadInt32(src, ref offset);
            posZ = BufferReader.ReadInt32(src, ref offset);
            eulerX = BufferReader.ReadInt32(src, ref offset);
            eulerY = BufferReader.ReadInt32(src, ref offset);
            eulerZ = BufferReader.ReadInt32(src, ref offset);
            velocityX = BufferReader.ReadInt32(src, ref offset);
            velocityY = BufferReader.ReadInt32(src, ref offset);
            velocityZ = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByte(result, entityId, ref offset);
            BufferWriter.WriteInt32(result, roleState, ref offset);
            BufferWriter.WriteInt32(result, posX, ref offset);
            BufferWriter.WriteInt32(result, posY, ref offset);
            BufferWriter.WriteInt32(result, posZ, ref offset);
            BufferWriter.WriteInt32(result, eulerX, ref offset);
            BufferWriter.WriteInt32(result, eulerY, ref offset);
            BufferWriter.WriteInt32(result, eulerZ, ref offset);
            BufferWriter.WriteInt32(result, velocityX, ref offset);
            BufferWriter.WriteInt32(result, velocityY, ref offset);
            BufferWriter.WriteInt32(result, velocityZ, ref offset);
            return result;
        }
    }

}
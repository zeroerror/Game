using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class WRoleStateUpdateMsg :IZeroMessage<WRoleStateUpdateMsg>{
        public int serverFrameIndex;
        public byte wRid;
        public int roleState;
        public int x; //(8  8)  (8  8)   整数部16位 int16（-32768 --- +32767） 小数部分16位 uint16(0 --- +65535) 0.0000到0.9999
        public int y;
        public int z;
        public bool isOwner;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrameIndex = BufferReader.ReadInt32(src, ref offset);
            wRid = BufferReader.ReadByte(src, ref offset);
            roleState = BufferReader.ReadInt32(src, ref offset);
            x = BufferReader.ReadInt32(src, ref offset);
            y = BufferReader.ReadInt32(src, ref offset);
            z = BufferReader.ReadInt32(src, ref offset);
            isOwner = BufferReader.ReadBool(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrameIndex, ref offset);
            BufferWriter.WriteByte(result, wRid, ref offset);
            BufferWriter.WriteInt32(result, roleState, ref offset);
            BufferWriter.WriteInt32(result, x, ref offset);
            BufferWriter.WriteInt32(result, y, ref offset);
            BufferWriter.WriteInt32(result, z, ref offset);
            BufferWriter.WriteBool(result, isOwner, ref offset);
            return result;
        }
    }

}
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class WorldRoomCreateResMessage :IZeroMessage<WorldRoomCreateResMessage>{
        public int roomEntityId;
        public string roomName;
        public string masterAccount;
        public string host; //战斗服host
        public ushort port; //战斗服端口

        public void FromBytes(byte[] src, ref int offset)
        {
            roomEntityId = BufferReader.ReadInt32(src, ref offset);
            roomName = BufferReader.ReadUTF8String(src, ref offset);
            masterAccount = BufferReader.ReadUTF8String(src, ref offset);
            host = BufferReader.ReadUTF8String(src, ref offset);
            port = BufferReader.ReadUInt16(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, roomEntityId, ref offset);
            BufferWriter.WriteUTF8String(result, roomName, ref offset);
            BufferWriter.WriteUTF8String(result, masterAccount, ref offset);
            BufferWriter.WriteUTF8String(result, host, ref offset);
            BufferWriter.WriteUInt16(result, port, ref offset);
            return result;
        }
    }

}
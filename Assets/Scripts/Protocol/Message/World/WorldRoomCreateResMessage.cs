using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class WorldRoomCreateResMessage :IZeroMessage<WorldRoomCreateResMessage>{
        public int roomEntityId;
        public string roomName;
        public string masterAccount;

        public void FromBytes(byte[] src, ref int offset)
        {
            roomEntityId = BufferReader.ReadInt32(src, ref offset);
            roomName = BufferReader.ReadUTF8String(src, ref offset);
            masterAccount = BufferReader.ReadUTF8String(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, roomEntityId, ref offset);
            BufferWriter.WriteUTF8String(result, roomName, ref offset);
            BufferWriter.WriteUTF8String(result, masterAccount, ref offset);
            return result;
        }
    }

}
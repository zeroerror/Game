using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class WorldCreateRoomReqMsg :IZeroMessage<WorldCreateRoomReqMsg>{
        public string roomName;

        public void FromBytes(byte[] src, ref int offset)
        {
            roomName = BufferReader.ReadUTF8String(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUTF8String(result, roomName, ref offset);
            return result;
        }

    }

}
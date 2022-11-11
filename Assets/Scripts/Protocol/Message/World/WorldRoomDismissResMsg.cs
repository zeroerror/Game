using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class WorldRoomDismissResMsg :IZeroMessage<WorldRoomDismissResMsg>{
        public int roomEntityID;

        public void FromBytes(byte[] src, ref int offset)
        {
            roomEntityID = BufferReader.ReadInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, roomEntityID, ref offset);
            return result;
        }
    }

}
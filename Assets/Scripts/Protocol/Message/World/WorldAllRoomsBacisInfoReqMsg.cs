using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class WorldAllRoomsBacisInfoReqMsg :IZeroMessage<WorldAllRoomsBacisInfoReqMsg>{

        public void FromBytes(byte[] src, ref int offset)
        {
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            return result;
        }

    }

}
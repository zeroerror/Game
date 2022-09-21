using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Client2World
{

    [ZeroMessage]
    public class WolrdEnterReqMessage:IZeroMessage<WolrdEnterReqMessage>{
        public byte battleRoleTypeId;
        public byte battleFieldId;

        public void FromBytes(byte[] src, ref int offset)
        {
            battleRoleTypeId = BufferReader.ReadByte(src, ref offset);
            battleFieldId = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, battleRoleTypeId, ref offset);
            BufferWriter.WriteByte(result, battleFieldId, ref offset);
            return result;
        }

    }

}
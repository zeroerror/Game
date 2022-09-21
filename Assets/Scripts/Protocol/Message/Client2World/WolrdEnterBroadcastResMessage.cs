using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Client2World
{

    [ZeroMessage]
    public class WolrdEnterBroadcastResMessage:IZeroMessage<WolrdEnterBroadcastResMessage>{
        public string account;
        public byte battleRoleTypeId;
        public byte battleFieldId;

        public void FromBytes(byte[] src, ref int offset)
        {
            account = BufferReader.ReadUTF8String(src, ref offset);
            battleRoleTypeId = BufferReader.ReadByte(src, ref offset);
            battleFieldId = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUTF8String(result, account, ref offset);
            BufferWriter.WriteByte(result, battleRoleTypeId, ref offset);
            BufferWriter.WriteByte(result, battleFieldId, ref offset);
            return result;
        }
    }

}
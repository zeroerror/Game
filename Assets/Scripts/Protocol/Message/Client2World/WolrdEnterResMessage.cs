using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Client2World
{

    [ZeroMessage]
    public class WolrdEnterResMessage :IZeroMessage<WolrdEnterResMessage>{
        public string account;
        public int entityId;
        public bool isOwner;

        public void FromBytes(byte[] src, ref int offset)
        {
            account = BufferReader.ReadUTF8String(src, ref offset);
            entityId = BufferReader.ReadInt32(src, ref offset);
            isOwner = BufferReader.ReadBool(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUTF8String(result, account, ref offset);
            BufferWriter.WriteInt32(result, entityId, ref offset);
            BufferWriter.WriteBool(result, isOwner, ref offset);
            return result;
        }
    }

}
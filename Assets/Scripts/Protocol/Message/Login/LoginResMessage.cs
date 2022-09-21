using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Login
{
    [ZeroMessage]
    public class LoginResMessage :IZeroMessage<LoginResMessage>{

        public byte status;
        public string userToken;
        public string[] worldServerHosts;
        public ushort[] worldServerPorts;

        public void FromBytes(byte[] src, ref int offset)
        {
            status = BufferReader.ReadByte(src, ref offset);
            userToken = BufferReader.ReadUTF8String(src, ref offset);
            worldServerHosts = BufferReader.ReadUTF8StringArray(src, ref offset);
            worldServerPorts = BufferReader.ReadUInt16Array(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, status, ref offset);
            BufferWriter.WriteUTF8String(result, userToken, ref offset);
            BufferWriter.WriteUTF8StringArray(result, worldServerHosts, ref offset);
            BufferWriter.WriteUInt16Array(result, worldServerPorts, ref offset);
            return result;
        }
    }

}
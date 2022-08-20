using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Login
{

    [ZeroMessage]
    public class RegisterAccountReqMessage :IZeroMessage<RegisterAccountReqMessage>{

        public string name;
        public string pwd;

        public void FromBytes(byte[] src, ref int offset)
        {
            name = BufferReader.ReadUTF8String(src, ref offset);
            pwd = BufferReader.ReadUTF8String(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUTF8String(result, name, ref offset);
            BufferWriter.WriteUTF8String(result, pwd, ref offset);
            return result;
        }

    }

}
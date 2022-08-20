using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.Login
{

    [ZeroMessage]
    public class RegisterAccountResMessage
:IZeroMessage<RegisterAccountResMessage>{

        public byte status;

        public void FromBytes(byte[] src, ref int offset)
        {
            status = BufferReader.ReadByte(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteByte(result, status, ref offset);
            return result;
        }


    }

}
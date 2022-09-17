using System;
using ZeroFrame.Protocol;
using ZeroFrame.Buffer;
namespace Game.Protocol.World
{

    [ZeroMessage]
    public class FrameWeaponAssetsSpawnResMsg
: IZeroMessage<FrameWeaponAssetsSpawnResMsg>
    {
        public int serverFrame;
        public byte[] weaponTypeArray;
        public ushort[] weaponIdArray;

        public void FromBytes(byte[] src, ref int offset)
        {
            serverFrame = BufferReader.ReadInt32(src, ref offset);
            weaponTypeArray = BufferReader.ReadByteArray(src, ref offset);
            weaponIdArray = BufferReader.ReadUInt16Array(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32(result, serverFrame, ref offset);
            BufferWriter.WriteByteArray(result, weaponTypeArray, ref offset);
            BufferWriter.WriteUInt16Array(result, weaponIdArray, ref offset);
            return result;
        }
    }

}
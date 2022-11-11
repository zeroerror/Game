using ZeroFrame.Protocol;
using ZeroFrame.Buffer;namespace Game.Protocol.World
{

    [ZeroMessage]
    public class WorldAllRoomsBacisInfoResMsg :IZeroMessage<WorldAllRoomsBacisInfoResMsg>{

        public int[] worldRoomIDArray;
        public string[] worldRoomNameArray;
        public int[] worldRoomMemNums;
        public int[] masterIDArray;
        public string[] hosts;
        public ushort[] ports;

        public void FromBytes(byte[] src, ref int offset)
        {
            worldRoomIDArray = BufferReader.ReadInt32Array(src, ref offset);
            worldRoomNameArray = BufferReader.ReadUTF8StringArray(src, ref offset);
            worldRoomMemNums = BufferReader.ReadInt32Array(src, ref offset);
            masterIDArray = BufferReader.ReadInt32Array(src, ref offset);
            hosts = BufferReader.ReadUTF8StringArray(src, ref offset);
            ports = BufferReader.ReadUInt16Array(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteInt32Array(result, worldRoomIDArray, ref offset);
            BufferWriter.WriteUTF8StringArray(result, worldRoomNameArray, ref offset);
            BufferWriter.WriteInt32Array(result, worldRoomMemNums, ref offset);
            BufferWriter.WriteInt32Array(result, masterIDArray, ref offset);
            BufferWriter.WriteUTF8StringArray(result, hosts, ref offset);
            BufferWriter.WriteUInt16Array(result, ports, ref offset);
            return result;
        }

    }

}
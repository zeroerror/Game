using ZeroFrame.Protocol;

namespace Game.Protocol.Client2World
{

    public class WolrdEnterBroadcastResMessage : IZeroMessage<WolrdEnterBroadcastResMessage>
    {
        public string account;
        public byte worldRoleTypeId;
        public byte worldFieldId;

        public void FromBytes(byte[] src, ref int offset)
        {
            throw new System.NotImplementedException();
        }

        public byte[] ToBytes()
        {
            throw new System.NotImplementedException();
        }
    }

}
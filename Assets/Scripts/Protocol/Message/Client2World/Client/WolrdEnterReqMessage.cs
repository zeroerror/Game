using ZeroFrame.Protocol;

namespace Game.Protocol.Client2World
{

    public class WolrdEnterReqMessage : IZeroMessage<WolrdEnterReqMessage>
    {
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
using System.Collections.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WorldRoomEntity
    {

        int masterID;
        public int MasterID => masterID;
        public void SetMasterID(int v) => masterID = v;

        int entityID;
        public int EntityID => entityID;
        public void SetEntityID(int v) => entityID = v;

        string roomName;
        public string RoomName => roomName;
        public void SetRoomName(string v) => roomName = v;

        string host;
        public string Host => host;
        public void SetHost(string v) => host = v;

        ushort port;
        public ushort Port => port;
        public void SetPort(ushort v) => port = v;

        Dictionary<int, string> memberDic;

        public WorldRoomEntity()
        {
            memberDic = new Dictionary<int, string>();
        }

        public void AddMember(int wroleID, string account)
        {
            if (!memberDic.TryGetValue(wroleID, out var mem))
            {
                return;
            };

            memberDic[wroleID] = account;
        }

        public void RemoveMember(int wroleID, string account)
        {
            memberDic.Remove(wroleID);
        }

        public int GetMemberNum()
        {
            return memberDic.Count;
        }

    }

}

using Game.Server.Bussiness.WorldBussiness.Network;
using Game.Infrastructure.Network.Server;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Server.Bussiness.WorldBussiness.Facades
{

    public class AllWorldNetwork
    {

        // Network
        public WorldRoleReqAndRes WorldRoleReqAndRes { get; private set; }
        public BulletReqAndRes BulletReqAndRes { get; private set; }
        public WeaponReqAndRes WeaponReqAndRes { get; private set; }
        public ItemReqAndRes ItemReqAndRes { get; private set; }

        public List<int> connIdList { get; private set; }

        int serverFrame;
        public int ServeFrame => serverFrame;

        public AllWorldNetwork()
        {
            WorldRoleReqAndRes = new WorldRoleReqAndRes();
            BulletReqAndRes = new BulletReqAndRes();
            WeaponReqAndRes = new WeaponReqAndRes();
            ItemReqAndRes = new ItemReqAndRes();

            connIdList = new List<int>();
        }

        public void Inject(NetworkServer server)
        {
            WorldRoleReqAndRes.Inject(server);
            BulletReqAndRes.Inject(server);
            WeaponReqAndRes.Inject(server);
            ItemReqAndRes.Inject(server);
        }

        public void Tick()
        {
            int totalSendCount = 0;
            totalSendCount += WorldRoleReqAndRes.SendCount;
            totalSendCount += BulletReqAndRes.SendCount;
            totalSendCount += WeaponReqAndRes.SendCount;
            totalSendCount += ItemReqAndRes.SendCount;
            if (totalSendCount > 0)
            {
                serverFrame++;
                WorldRoleReqAndRes.ClearSendCount();
                BulletReqAndRes.ClearSendCount();
                WeaponReqAndRes.ClearSendCount();
                ItemReqAndRes.ClearSendCount();

                WorldRoleReqAndRes.SetServerFrame(serverFrame);
                BulletReqAndRes.SetServerFrame(serverFrame);
                WeaponReqAndRes.SetServerFrame(serverFrame);
                ItemReqAndRes.SetServerFrame(serverFrame);
                Debug.Log($"状态帧更新--------------------------> {serverFrame}");
            }
        }

    }

}
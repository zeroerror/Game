
using Game.Server.Bussiness.BattleBussiness.Network;
using Game.Infrastructure.Network.Server;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Server.Bussiness.BattleBussiness.Facades
{

    public class AllBattleNetwork
    {

        // Network
        public BattleReqAndRes BattleReqAndRes { get; private set; }
        public BattleRoleReqAndRes RoleReqAndRes { get; private set; }
        public BulletReqAndRes BulletReqAndRes { get; private set; }
        public WeaponReqAndRes WeaponReqAndRes { get; private set; }
        public ItemReqAndRes ItemReqAndRes { get; private set; }

        public List<int> connIdList { get; private set; }

        public NetworkServer BattleServer { get; private set; }

        int serverFrame;
        public int ServeFrame => serverFrame;

        public AllBattleNetwork()
        {
            BattleReqAndRes = new BattleReqAndRes();
            RoleReqAndRes = new BattleRoleReqAndRes();
            BulletReqAndRes = new BulletReqAndRes();
            WeaponReqAndRes = new WeaponReqAndRes();
            ItemReqAndRes = new ItemReqAndRes();

            connIdList = new List<int>();
        }

        public void Inject(NetworkServer server)
        {
            BattleServer = server;
            BattleReqAndRes.Inject(server);
            RoleReqAndRes.Inject(server);
            BulletReqAndRes.Inject(server);
            WeaponReqAndRes.Inject(server);
            ItemReqAndRes.Inject(server);
        }

        public void Tick()
        {
            int totalSendCount = 0;
            totalSendCount += BattleReqAndRes.SendCount;
            totalSendCount += RoleReqAndRes.SendCount;
            totalSendCount += BulletReqAndRes.SendCount;
            totalSendCount += WeaponReqAndRes.SendCount;
            totalSendCount += ItemReqAndRes.SendCount;
            if (totalSendCount > 0)
            {
                BattleReqAndRes.ClearSendCount();
                RoleReqAndRes.ClearSendCount();
                BulletReqAndRes.ClearSendCount();
                WeaponReqAndRes.ClearSendCount();
                ItemReqAndRes.ClearSendCount();

                serverFrame++;
                // Debug.Log($"状态帧更新--------------------------> {serverFrame}");

                BattleReqAndRes.SetServerFrame(serverFrame);
                RoleReqAndRes.SetServerFrame(serverFrame);
                BulletReqAndRes.SetServerFrame(serverFrame);
                WeaponReqAndRes.SetServerFrame(serverFrame);
                ItemReqAndRes.SetServerFrame(serverFrame);
            }

            BattleReqAndRes.TickAllRegistAction();
            RoleReqAndRes.TickAllRegistAction();
            BulletReqAndRes.TickAllRegistAction();
            WeaponReqAndRes.TickAllRegistAction();
            ItemReqAndRes.TickAllRegistAction();
        }

    }

}
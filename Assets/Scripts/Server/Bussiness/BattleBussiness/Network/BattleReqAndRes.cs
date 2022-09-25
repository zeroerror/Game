

using System;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Server;
using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;

namespace Game.Server.Bussiness.BattleBussiness.Network
{

    public class BattleReqAndRes
    {
        NetworkServer _server;

        public BattleReqAndRes()
        {

        }

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        // ====== Send ======
        public void SendRes_ConnectSuccess()
        {

        }

        // ====== Regist ======


    }

}
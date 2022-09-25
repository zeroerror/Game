

using System;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Network
{

    public class BattleReqAndRes
    {
        NetworkClient battleClient;

        public BattleReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            battleClient = client;
        }

        public void ConnBattleServer(string host, ushort port)
        {
            Debug.Log($"尝试连接战斗服:{host}:{port}");
            battleClient.Connect(host, port);
        }

    }

}
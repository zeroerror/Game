using System;
using UnityEngine;
using Game.Infrastructure.Network.Client;
using Game.Protocol.Battle;
using Game.Protocol.Client2World;
using System.Threading;

namespace Game.Client.Bussiness.WorldBussiness.Network
{

    public class WorldReqAndRes
    {
        NetworkClient _worldServClient;

        public WorldReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            _worldServClient = client;
        }

        // == Send ==
        public void ConnWorldServer(string host, ushort port)
        {
            Debug.Log($"尝试连接世界服:{host}:{port}");
            _worldServClient.Connect(host, port);
        }

        // == Regist ==

        public void RegistRes_WorldEnter(Action<WolrdEnterResMessage> action)
        {
            _worldServClient.RegistMsg<WolrdEnterResMessage>(action);
        }

    }

}
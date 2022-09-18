using System;
using UnityEngine;
using Game.Protocol.World;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness.Network
{

    public class WeaponReqAndRes
    {
        NetworkClient _client;

        public WeaponReqAndRes()
        {

        }

        public void Inject(NetworkClient client)
        {
            _client = client;
        }

        public void RegistRes_WeaponAssetsSpawn(Action<FrameItemSpawnResMsg> action)
        {
            _client.RegistMsg<FrameItemSpawnResMsg>(action);
        }

    }

}
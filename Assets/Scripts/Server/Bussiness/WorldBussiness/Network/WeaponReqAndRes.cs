using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.World;
using Game.Client.Bussiness.WorldBussiness;

namespace Game.Server.Bussiness.WorldBussiness.Network
{

    public class WeaponReqAndRes
    {
        NetworkServer _server;

        public WeaponReqAndRes()
        {

        }

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

     

    }

}
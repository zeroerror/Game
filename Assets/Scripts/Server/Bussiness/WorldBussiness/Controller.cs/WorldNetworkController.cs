using System.Collections.Generic;
using UnityEngine;
using Game.Protocol.World;
using Game.Infrastructure.Generic;
using Game.Client.Bussiness.WorldBussiness;
using Game.Server.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.WorldBussiness.Generic;

namespace Game.Server.Bussiness.WorldBussiness
{

    public class WorldNetworkController
    {

        WorldFacades worldFacades;
        int serveFrame;

        public void Inject(WorldFacades worldFacades, float fixedDeltaTime)
        {
            this.worldFacades = worldFacades;
        }

        public void Tick()
        {
            worldFacades.Network.Tick();
        }

    }

}
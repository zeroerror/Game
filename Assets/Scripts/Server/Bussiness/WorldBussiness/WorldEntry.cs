using System.Threading;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Server;
using Game.Server.Bussiness.EventCenter;
using Game.Server.Bussiness.WorldBussiness.Controller;
using Game.Server.Bussiness.WorldBussiness.Facades;
using UnityEngine;

namespace Game.Server.Bussiness.WorldBussiness
{

    public class WorldEntry
    {

        // Facades
        WorldFacades worldFacades;

        // Controller
        WorldController worldController;

        
        #region [Life Cycle]

        public WorldEntry()
        {
            // == Facades ==
            worldFacades = new WorldFacades();
            // == Controller ==
            worldController = new WorldController();
        }

        public void Inject(NetworkServer server)
        {
            // == Facades ==
            worldFacades.Inject(server);
            worldController.Inject(worldFacades);
            // == Controller ==
        }

        public void Tick()
        {
            // == Controller ==
            worldController.Tick();
        }

        public static void TearDown()
        {

        }

        #endregion

        
    }

}
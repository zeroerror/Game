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
        ServerWorldFacades serverWorldFacades;

        // Controller
        WorldController worldController;


        #region [Life Cycle]

        public WorldEntry()
        {
            // == Controller ==
            worldController = new WorldController();
        }

        public void Inject(ServerWorldFacades facades)
        {
            // == Facades ==
            serverWorldFacades = facades;
            // == Controller ==
            worldController.Inject(serverWorldFacades);
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
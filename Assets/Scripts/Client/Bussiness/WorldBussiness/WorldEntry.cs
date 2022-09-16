using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.WorldBussiness.Controller;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness
{

    public static class WorldEntry
    {


        // Facades
        static WorldFacades worldFacades;

        // Controller
        static WorldController worldController;

        #region [Life Cycle]

        public static void Ctor()
        {
            // == Facades ==
            worldFacades = new WorldFacades();
            // == Controller ==
            worldController = new WorldController();
        }

        public static void Init()
        {
            worldFacades.Init();
        }

        public static void Inject(NetworkClient client, InputComponent inputComponent)
        {
            // == Facades ==
            worldFacades.Inject(client, inputComponent);
            // == Controller ==
            worldController.Inject(worldFacades);
        }

        public static void Tick()
        {
            // == Controller ==
            worldController.Tick();
        }

        public static void Update()
        {
            float deltaTime = UnityEngine.Time.deltaTime;
            worldController.Update_RoleRenderer(deltaTime);
            worldController.Update_Camera();
        }

        public static void TearDown()
        {

        }

        #endregion

    }

}
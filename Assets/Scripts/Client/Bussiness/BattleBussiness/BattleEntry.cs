using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Controller;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.BattleBussiness
{

    public static class BattleEntry
    {

        // Facades
        static BattleFacades battleFacades;

        // Controller
        static BattleController battleController;
        static BattlePhysicsController battlePhysicsController;
        static BattleInputController battleInputController;
        static BattleWeaponController battleWeaponController;
        static BattleRendererController battleRendererController;
        static BattleNetworkController battleNetworkController;

        #region [Life Cycle]

        public static void Ctor()
        {
            // == Facades ==
            battleFacades = new BattleFacades();
            // == Controller ==
            battleController = new BattleController();
            battlePhysicsController = new BattlePhysicsController();
            battleInputController = new BattleInputController();
            battleWeaponController = new BattleWeaponController();
            battleRendererController = new BattleRendererController();
            battleNetworkController = new BattleNetworkController();
        }

        public static void Inject(NetworkClient client, PlayerInputComponent inputComponent)
        {
            // == Facades ==
            battleFacades.Inject(client, inputComponent);
            // == Controller ==
            battleController.Inject(battleFacades);
            battlePhysicsController.Inject(battleFacades);
            battleInputController.Inject(battleFacades);
            battleWeaponController.Inject(battleFacades);
            battleRendererController.Inject(battleFacades);
            battleNetworkController.Inject(battleFacades);
        }

        public static void Tick()
        {
            // == Controller ==
            battleNetworkController.Tick();

            battleController.Tick();
            battlePhysicsController.Tick();
            battleInputController.Tick();
            battleWeaponController.Tick();
            battleRendererController.Tick();
        }

        public static void Update()
        {
            // Renderer
            float deltaTime = UnityEngine.Time.deltaTime;
            battleRendererController.Update(deltaTime);
        }

        public static void TearDown()
        {

        }

        #endregion

    }

}
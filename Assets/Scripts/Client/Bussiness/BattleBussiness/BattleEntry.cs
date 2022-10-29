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
        static BattleBulletController BattleBulletController;
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
            BattleBulletController = new BattleBulletController();
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
            BattleBulletController.Inject(battleFacades);
            battlePhysicsController.Inject(battleFacades);
            battleInputController.Inject(battleFacades);
            battleWeaponController.Inject(battleFacades);
            battleRendererController.Inject(battleFacades);
            battleNetworkController.Inject(battleFacades);
        }

        public static void Tick(float fixedDeltaTime)
        {
            // == Controller ==
            battleNetworkController.Tick();

            battleController.Tick(fixedDeltaTime);
            BattleBulletController.Tick(fixedDeltaTime);
            battlePhysicsController.Tick();
            battleInputController.Tick();
            battleWeaponController.Tick();
            
            battleRendererController.Tick();    // Last But No Least
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
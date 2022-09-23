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

        #region [Life Cycle]

        public static void Ctor()
        {
            // == Facades ==
            battleFacades = new BattleFacades();
            // == Controller ==
            battleController = new BattleController();
            battlePhysicsController = new BattlePhysicsController();
            battleInputController = new BattleInputController();
        }

        public static void Inject(NetworkClient client, InputComponent inputComponent)
        {
            // == Facades ==
            battleFacades.Inject(client, inputComponent);
            // == Controller ==
            battleController.Inject(battleFacades);
            battlePhysicsController.Inject(battleFacades);
            battleInputController.Inject(battleFacades);
        }

        public static void Tick()
        {
            // == Controller ==
            battleController.Tick();
            battlePhysicsController.Tick();
            battleInputController.Tick();
        }

        public static void Update()
        {
            float deltaTime = UnityEngine.Time.deltaTime;
            battleController.Update_RoleRenderer(deltaTime);
            battleController.Update_Camera();
        }

        public static void TearDown()
        {

        }

        #endregion

    }

}
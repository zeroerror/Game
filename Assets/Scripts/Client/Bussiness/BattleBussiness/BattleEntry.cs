using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Controller;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleEntry
    {

        // Facades
        BattleFacades battleFacades;

        // Controller
        BattleController battleController;
        BattleBulletController BattleBulletController;
        BattlePhysicsController battlePhysicsController;
        BattleInputController battleInputController;
        BattleWeaponController battleWeaponController;
        BattleRendererController battleRendererController;
        BattleNetworkController battleNetworkController;
        BattleArmorController battleArmorController;

        #region [Life Cycle]
        public BattleEntry() { }

        public void Ctor()
        {
            // == Controller ==
            battleController = new BattleController();
            BattleBulletController = new BattleBulletController();
            battlePhysicsController = new BattlePhysicsController();
            battleInputController = new BattleInputController();
            battleWeaponController = new BattleWeaponController();
            battleRendererController = new BattleRendererController();
            battleNetworkController = new BattleNetworkController();
            battleArmorController = new BattleArmorController();
        }

        public void Inject(BattleFacades battleFacades)
        {
            // == Controller ==
            battleController.Inject(battleFacades);
            BattleBulletController.Inject(battleFacades);
            battlePhysicsController.Inject(battleFacades);
            battleInputController.Inject(battleFacades);
            battleWeaponController.Inject(battleFacades);
            battleRendererController.Inject(battleFacades);
            battleNetworkController.Inject(battleFacades);
            battleArmorController.Inject(battleFacades);
        }

        public void Tick(float fixedDeltaTime)
        {
            // == Controller ==
            battleNetworkController.Tick();

            battleController.Tick(fixedDeltaTime);
            BattleBulletController.Tick(fixedDeltaTime);
            battlePhysicsController.Tick(fixedDeltaTime);
            battleInputController.Tick(fixedDeltaTime);
            battleWeaponController.Tick(fixedDeltaTime);
            battleArmorController.Tick(fixedDeltaTime);
        }

        public void Update()
        {
            float deltaTime = UnityEngine.Time.deltaTime;
            battleRendererController.Update(deltaTime);
        }

        public void TearDown()
        {

        }

        #endregion

    }

}
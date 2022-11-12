using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleEntry
    {
        ServerBattleFacades serverFacades;

        BattleController battleController;
        BattlePhysicsController battlePhysicsController;
        BattleNetworkController battleNetworkController;
        BattleWeaponController battleWeaponController;
        BattleLifeController battleLifeController;
        BattleArmorController battleArmorController;
        BattleBulletController battleBulletController;

        public BattleEntry()
        {
            battleController = new BattleController();
            battlePhysicsController = new BattlePhysicsController();
            battleNetworkController = new BattleNetworkController();
            battleWeaponController = new BattleWeaponController();
            battleLifeController = new BattleLifeController();
            battleArmorController = new BattleArmorController();
            battleBulletController = new BattleBulletController();
        }

        public void Inject(ServerBattleFacades facades)
        {
            // Facades
            this.serverFacades = facades;
            // Conntroller
            battleController.Inject(serverFacades);
            battlePhysicsController.Inject(serverFacades);
            battleNetworkController.Inject(serverFacades);
            battleWeaponController.Inject(serverFacades);
            battleLifeController.Inject(serverFacades);
            battleArmorController.Inject(serverFacades);
            battleBulletController.Inject(serverFacades);
        }

        public void Tick(float fixedDeltaTime)
        {
            // - Game State
            var gameStateDomain = serverFacades.BattleFacades.Domain.BattleStateDomain;
            gameStateDomain.ApplyGameState();

            var gameEntity = serverFacades.BattleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var state = fsm.BattleState;
            if (!state.CanBattleLoop())
            {
                return;
            }

            battleNetworkController.Tick();
            battleController.Tick(fixedDeltaTime);
            battlePhysicsController.Tick(fixedDeltaTime);
            battleWeaponController.Tick(fixedDeltaTime);
            battleLifeController.Tick(fixedDeltaTime);
            battleArmorController.Tick(fixedDeltaTime);
            battleBulletController.Tick(fixedDeltaTime);
        }

    }

}
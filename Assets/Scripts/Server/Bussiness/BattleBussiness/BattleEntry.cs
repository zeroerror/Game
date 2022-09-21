using Game.Infrastructure.Network.Server;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleEntry
    {
        BattleFacades battleFacades;

        BattleController battleController;
        BattlePhysicsController battlePhysicsController;
        BattleNetworkController battleNetworkController;

        public BattleEntry()
        {
            battleFacades = new BattleFacades();
            battleController = new BattleController();
            battlePhysicsController = new BattlePhysicsController();
            battleNetworkController = new BattleNetworkController();
        }

        public void Inject(NetworkServer server, float fixedDeltaTime)
        {
            // Facades
            battleFacades.Inject(server);

            // Conntroller
            battleController.Inject(battleFacades, fixedDeltaTime);
            battlePhysicsController.Inject(battleFacades, fixedDeltaTime);
            battleNetworkController.Inject(battleFacades, fixedDeltaTime);
        }

        public void Init()
        {
            battleFacades.ClientBattleFacades.Init();
        }

        public void Tick()
        {
            battleController.Tick();
            battlePhysicsController.Tick();
            battleNetworkController.Tick();
        }

    }

}
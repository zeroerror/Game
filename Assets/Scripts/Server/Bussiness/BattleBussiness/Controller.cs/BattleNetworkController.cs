using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleNetworkController
    {

        BattleFacades battleFacades;
        int serveFrame;

        public void Inject(BattleFacades battleFacades, float fixedDeltaTime)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick()
        {
            battleFacades.Network.Tick();
        }

    }

}
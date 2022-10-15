using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleNetworkController
    {

        BattleServerFacades battleFacades;
        int serveFrame;

        public void Inject(BattleServerFacades battleFacades, float fixedDeltaTime)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick()
        {
            battleFacades.Network.Tick();
        }

    }

}
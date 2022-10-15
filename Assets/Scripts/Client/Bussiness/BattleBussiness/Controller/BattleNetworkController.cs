using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleNetworkController
    {

        BattleFacades battleFacades;
        int serveFrame;

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick()
        {
            battleFacades.Network.Tick();
        }

    }

}
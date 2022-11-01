using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleNetworkController
    {

        BattleServerFacades battleFacades;

        public void Inject(BattleServerFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick()
        {
            battleFacades.Network.Tick();
        }

    }

}
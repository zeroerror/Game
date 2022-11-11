using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleNetworkController
    {

        ServerBattleFacades serverFacades;

        public void Inject(ServerBattleFacades v)
        {
            this.serverFacades = v;
        }

        public void Tick()
        {
            serverFacades.Network.Tick();
        }

    }

}
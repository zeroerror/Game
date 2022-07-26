using Game.Infrastructure.Input;
using Game.Client.Bussiness.WorldBussiness.Controller.Domain;
using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class WorldFacades
    {
        public AllWorldNetwork Network { get; private set; }
        public AllWorldRepo Repo { get; private set; }
        public AllDomains Domain { get; private set; }
        // Asset
        public AllWorldAssets Assets { get; private set; }
        // Controller Set
        public CinemachineExtra CinemachineExtra { get; private set; }
        public void SetCinemachineExtra(CinemachineExtra cinemachineExtra) => CinemachineExtra = cinemachineExtra;

        public WorldFacades()
        {
            Network = new AllWorldNetwork();
            Repo = new AllWorldRepo();
            Domain = new AllDomains();

            Assets = new AllWorldAssets();
        }

        public void Inject(NetworkClient client)
        {
            Domain.Inject(this);
            Network.Inject(client);
        }

        public void Init()
        {
            // Asset Load
            Assets.LoadAll();
        }

    }

}
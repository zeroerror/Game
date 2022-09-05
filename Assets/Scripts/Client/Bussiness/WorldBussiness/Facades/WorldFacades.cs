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
        public InputComponent InputComponent { get; private set; }

        public WorldFacades()
        {
            Network = new AllWorldNetwork();
            Repo = new AllWorldRepo();
            Domain = new AllDomains();
            Assets = new AllWorldAssets();
            Domain.Inject(this);
        }

        public void Inject(NetworkClient client, InputComponent inputComponent)
        {
            Network.Inject(client);
            InputComponent = inputComponent;
        }

        public void Init()
        {
            // Asset Load
            Assets.LoadAll();
        }

    }

}
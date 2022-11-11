using Game.Infrastructure.Network.Client;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class WorldFacades
    {

        public AllWorldNetwork Network { get; private set; }
        public AllWorldRepo Repo { get; private set; }

        public WorldFacades()
        {
            Network = new AllWorldNetwork();
            Repo = new AllWorldRepo();
        }

        public void Inject(NetworkClient client)
        {
            Network.Inject(client);
        }

        public void Init()
        {
        }

    }

}
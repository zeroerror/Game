

using Game.Client.Bussiness.WorldBussiness.Repo;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllWorldRepo
    {

        public WorldRoleRepo WorldRoleRepo { get; private set; }

        public AllWorldRepo()
        {
            WorldRoleRepo = new WorldRoleRepo();
        }

    }


}
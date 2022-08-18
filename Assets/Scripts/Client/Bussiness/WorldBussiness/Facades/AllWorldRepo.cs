using Game.Client.Bussiness.WorldBussiness.Repo;
using Game.Client.Bussiness.Repo;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllWorldRepo
    {

        public WorldRoleRepo WorldRoleRepo { get; private set; }
        public FiledEntityRepo FiledEntityRepo { get; private set; }

        public AllWorldRepo()
        {
            WorldRoleRepo = new WorldRoleRepo();
            FiledEntityRepo = new FiledEntityRepo();
        }

    }

}
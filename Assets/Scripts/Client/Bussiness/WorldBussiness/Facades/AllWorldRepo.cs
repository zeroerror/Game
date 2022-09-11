using Game.Client.Bussiness.WorldBussiness.Repo;
using Game.Client.Bussiness.Repo;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllWorldRepo
    {

        public FiledEntityRepo FiledEntityRepo { get; private set; }
        public WorldRoleRepo WorldRoleRepo { get; private set; }
        public BulletEntityRepo BulletRepo { get; private set; }

        public AllWorldRepo()
        {
            FiledEntityRepo = new FiledEntityRepo();
            WorldRoleRepo = new WorldRoleRepo();
            BulletRepo = new BulletEntityRepo();
        }

    }

}
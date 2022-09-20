using Game.Client.Bussiness.WorldBussiness.Repo;
using Game.Client.Bussiness.Repo;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllWorldRepo
    {

        public FiledEntityRepo FiledRepo { get; private set; }
        public WorldRoleRepo RoleRepo { get; private set; }
        public WeaponRepo WeaponRepo { get; private set; }
        public BulletRepo BulletRepo { get; private set; }
        public BulletPackRepo BulletPackRepo { get; private set; }

        public AllWorldRepo()
        {
            FiledRepo = new FiledEntityRepo();
            RoleRepo = new WorldRoleRepo();
            WeaponRepo = new WeaponRepo();
            BulletRepo = new BulletRepo();
            BulletPackRepo = new BulletPackRepo();
        }

    }

}


using Game.Client.Bussiness.WorldBussiness.Repo;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllWorldRepo
    {

        public WorldRoleRepo WorldRoleRepo { get; private set; }
        public WorldRoomRepo WorldRoomRepo { get; private set; }

        public AllWorldRepo()
        {
            WorldRoleRepo = new WorldRoleRepo();
            WorldRoomRepo = new WorldRoomRepo();
        }

    }


}
using Game.Client.Bussiness.WorldBussiness.Assets;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllWorldAssets
    {

        public WorldRoleAssets WorldRoleAssets { get; private set; }

        public AllWorldAssets()
        {
            WorldRoleAssets = new WorldRoleAssets();
        }

        public void LoadAll()
        {
            WorldRoleAssets.LoadAssets();
        }

    }

}
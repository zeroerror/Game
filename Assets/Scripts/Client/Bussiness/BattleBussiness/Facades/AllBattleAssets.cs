using System;
using Game.Client.Bussiness.Assets;
using Game.Client.Bussiness.BattleBussiness.Assets;

namespace Game.Client.Bussiness.BattleBussiness.Facades
{

    public class AllBattleAssets
    {

        public BattleRoleAssets BattleRoleAssets { get; private set; }
        public ItemAsset ItemAsset { get; private set; }
        public BulletAsset BulletAsset { get; private set; }
        public CameraAsset CameraAsset { get; private set; }

        public AllBattleAssets()
        {
            BattleRoleAssets = new BattleRoleAssets();
            ItemAsset = new ItemAsset();
            BulletAsset = new BulletAsset();
            CameraAsset = new CameraAsset();
        }

        public void LoadAll()
        {
            Console.WriteLine("世界资源开始加载------------------------------------------");
            BattleRoleAssets.LoadAssets();
            ItemAsset.LoadAssets();
            BulletAsset.LoadAssets();
            CameraAsset.LoadAssets();
        }

    }

}
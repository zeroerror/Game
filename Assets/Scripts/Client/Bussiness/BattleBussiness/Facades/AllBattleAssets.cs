using System;
using System.Threading.Tasks;
using Game.Client.Bussiness.Assets;
using Game.Client.Bussiness.BattleBussiness.Assets;

namespace Game.Client.Bussiness.BattleBussiness.Facades
{

    public class AllBattleAssets
    {

        public BattleRoleAssets BattleRoleAssets { get; private set; }
        public WeaponAsset WeaponAsset { get; private set; }
        public ArmorAsset ArmorAsset { get; private set; }
        public BulletAsset BulletAsset { get; private set; }
        public CameraAsset CameraAsset { get; private set; }

        public ItemAsset ItemAsset { get; private set; }
        public AllBattleAssets()
        {
            BattleRoleAssets = new BattleRoleAssets();
            WeaponAsset = new WeaponAsset();
            ArmorAsset = new ArmorAsset();
            BulletAsset = new BulletAsset();
            CameraAsset = new CameraAsset();
            ItemAsset = new ItemAsset();
        }

        public Task LoadAll()
        {
            Console.WriteLine("资源开始加载------------------------------------------");
            return new Task(() =>
            {
                BattleRoleAssets.LoadAssets();
                WeaponAsset.LoadAssets();
                ArmorAsset.LoadAssets();
                BulletAsset.LoadAssets();
                CameraAsset.LoadAssets();
                ItemAsset.LoadAssets();
            });

        }

    }

}
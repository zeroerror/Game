using System;
using Game.Client.Bussiness.Assets;
using Game.Client.Bussiness.WorldBussiness.Assets;

namespace Game.Client.Bussiness.WorldBussiness.Facades
{

    public class AllWorldAssets
    {

        public WorldRoleAssets WorldRoleAssets { get; private set; }
        public WeaponAsset WeaponAsset { get; private set; }
        public BulletAsset BulletAsset { get; private set; }
        public CameraAsset CameraAsset { get; private set; }

        public AllWorldAssets()
        {
            WorldRoleAssets = new WorldRoleAssets();
            WeaponAsset = new WeaponAsset();
            BulletAsset = new BulletAsset();
            CameraAsset = new CameraAsset();
        }

        public void LoadAll()
        {
            Console.WriteLine("世界资源开始加载------------------------------------------");
            WorldRoleAssets.LoadAssets();
            WeaponAsset.LoadAssets();
            BulletAsset.LoadAssets();
            CameraAsset.LoadAssets();
        }

    }

}
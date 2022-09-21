namespace Game.Client.Bussiness.BattleBussiness.Generic
{

    public enum ItemType
    {
        Default,
        Weapon,
        BulletPack,
        Pill    // heal or speedup ,etc.
    }

    public enum BulletType
    {
        DefaultBullet,
        Grenade,
        Hooker
    }

    public enum WeaponType
    {
        // TODO: 根据武器类型去配置表查询对应弹夹容量
        // TODO: 根据武器类型去配置表查询对应所需子弹类型
        Pistol, // 手枪
        Rifle,  // 步枪
        GrenadeLauncher //榴弹发射器
    }


}
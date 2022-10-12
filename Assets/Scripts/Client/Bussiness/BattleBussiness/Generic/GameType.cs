namespace Game.Client.Bussiness.BattleBussiness.Generic
{

    public enum EntityType : short
    {
        BattleRole,
        Bullet,
        Weapon,
        Item
    }

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
        Pistol, // 手枪
        Rifle,  // 步枪
        GrenadeLauncher //榴弹发射器
    }

}
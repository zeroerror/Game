using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.Interfaces;

namespace Game.Client.Bussiness.BattleBussiness
{

    public enum RoleState
    {
        Normal,
        Move,
        Jump,
        Hooking       //-使用爪钩中
    }

    public class BattleRoleLogicEntity : PhysicsEntity
    {
        public BattleRoleRendererEntity roleRenderer { get; private set; }
        public Vector3 SelfPos => transform.position;

        byte entityId;
        public byte EntityId => entityId;
        public void SetEntityId(byte entityId) => this.entityId = entityId;

        int connId;
        public int ConnId => connId;
        public void SetConnId(int connId) => this.connId = connId;

        Vector3 offset;
        Vector3 shootPointPos => MoveComponent.CurPos + transform.forward + offset;
        public Vector3 ShootPointPos => shootPointPos.FixDecimal(4);

        // == Component ==
        [SerializeField]
        public MoveComponent moveComponent;
        public MoveComponent MoveComponent => moveComponent;
        public HealthComponent HealthComponent { get; private set; }
        public WeaponComponent WeaponComponent { get; private set; }
        public ItemComponent ItemComponent { get; private set; }

        public RoleState RoleState { get; private set; }
        public void SetRoleState(RoleState roleStatus) => this.RoleState = roleStatus;

        public bool IsDead { get; private set; }

        public void Inject(BattleRoleRendererEntity roleRendererEntity)
        {
            roleRenderer = roleRendererEntity;
        }

        public void Ctor()
        {
            // == Component
            // moveComponent = new MoveComponent();
            moveComponent.Inject(transform.GetComponentInParent<Rigidbody>());
            moveComponent.SetMaximumSpeed(30f);

            HealthComponent = new HealthComponent(100f);

            WeaponComponent = new WeaponComponent();
            WeaponComponent.Ctor();

            ItemComponent = new ItemComponent();
            ItemComponent.Ctor();

            RoleState = RoleState.Normal;
            offset = new Vector3(0, 0.2f, 0);
        }

        public bool TryWeaponReload(out int realTakeOut)
        {
            realTakeOut = 0;
            var curWeapon = WeaponComponent.CurrentWeapon;
            if (curWeapon == null)
            {
                Debug.LogWarning("当前尚未持有武器！");
                return false;
            }

            // 获取武器所需子弹
            // TODO: 根据配置表查询武器对应所需子弹
            // var curWeapon = WeaponComponent.CurrentWeapon;
            // switch (curWeapon.WeaponType)
            // {
            //     case WeaponType.Pistol:
            //         break;
            //     case WeaponType.Rifle:
            //         break;
            //     case WeaponType.GrenadeLauncher:
            //         break;
            // }

            realTakeOut = ItemComponent.TryTakeOutItem_Bullet(curWeapon.BulletCapacity - curWeapon.bulletNum);
            if (realTakeOut == 0)
            {
                Debug.LogWarning($"武器[{curWeapon.name}]所需子弹不足！");
                return false;
            }

            Debug.Log($"武器换弹,取出子弹数量：{realTakeOut}");
            curWeapon.LoadBullet(realTakeOut);
            WeaponComponent.SetReloading(false);
            return true;
        }

        public void WeaponReload(int reloadBulletNum)
        {
            var curWeapon = WeaponComponent.CurrentWeapon;
            if (curWeapon == null)
            {
                Debug.LogWarning("当前尚未持有武器！");
                return;
            }

            var realTakeOut = ItemComponent.TryTakeOutItem_Bullet(reloadBulletNum);
            if (realTakeOut == 0)
            {
                Debug.LogWarning($"武器[{curWeapon.name}]所需子弹不足！");
                return;
            }

            Debug.Log($"武器换弹,取出子弹数量：{realTakeOut}");
            curWeapon.LoadBullet(realTakeOut);
            WeaponComponent.SetReloading(false);
            return;
        }

        public bool CanWeaponReload()
        {
            var curWeapon = WeaponComponent.CurrentWeapon;
            if (curWeapon == null)
            {
                Debug.LogWarning("当前尚未持有武器！");
                return false;
            }

            if (WeaponComponent.IsFullReloaded)
            {
                Debug.LogWarning("当前武器已经装满子弹");
                return false;
            }
            if (!ItemComponent.HasItem_Bullet(1))
            {
                Debug.LogWarning("当前1颗子弹都没了");
                return false;
            }

            return true;
        }

        public bool IsIdle()
        {
            return MoveComponent.Velocity == Vector3.zero;
        }

        public void TearDown()
        {
            IsDead = true;
        }

        public void Reborn()
        {
            Debug.Log($"重生 wRid:{entityId}");
            MoveComponent.Reset();
            HealthComponent.Reset();
            IsDead = false;
        }

        public bool IsAllowEnterNormal()
        {
            return true;
        }

    }

}
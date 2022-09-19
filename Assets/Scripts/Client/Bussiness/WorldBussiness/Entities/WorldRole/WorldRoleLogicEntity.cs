using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.Interfaces;

namespace Game.Client.Bussiness.WorldBussiness
{

    public enum RoleState
    {
        Normal,
        Move,
        Jump,
        Hooking       //-使用爪钩中
    }

    public class WorldRoleLogicEntity : PhysicsEntity
    {
        public WorldRoleRendererEntity roleRenderer { get; private set; }
        public Vector3 SelfPos => transform.position;

        byte wRid;
        public byte WRid => wRid;
        public void SetWRid(byte wRid) => this.wRid = wRid;

        int connId;
        public int ConnId => connId;
        public void SetConnId(int connId) => this.connId = connId;

        Vector3 offset;
        Vector3 shootPointPos => MoveComponent.CurPos + transform.forward + offset;
        public Vector3 ShootPointPos => shootPointPos.FixDecimal(4);

        // == Component ==
        public MoveComponent MoveComponent { get; private set; }
        public HealthComponent HealthComponent { get; private set; }
        public WeaponComponent WeaponComponent { get; private set; }
        public ItemComponent ItemComponent { get; private set; }

        public RoleState RoleState { get; private set; }
        public void SetRoleState(RoleState roleStatus) => this.RoleState = roleStatus;

        public bool IsDead { get; private set; }
        public bool IsOldState;

        public void Inject(WorldRoleRendererEntity roleRendererEntity)
        {
            roleRenderer = roleRendererEntity;
        }

        public void Ctor()
        {
            // == Component
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>(), 4f, 4f);
            MoveComponent.SetMaximumSpeed(30f);

            HealthComponent = new HealthComponent(100f);

            WeaponComponent = new WeaponComponent();
            WeaponComponent.Ctor();

            ItemComponent = new ItemComponent();
            ItemComponent.Ctor();

            RoleState = RoleState.Normal;
            offset = new Vector3(0, 0.2f, 0);
        }

        public void WeaponReload()
        {
            if (WeaponComponent.IsHoldingWeapon())
            {
                Debug.LogWarning("当前尚未持有武器！");
                //TODO: 徒手攻击
                return;
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
            int takeOut = ItemComponent.TryTakeOutItem_Bullet(30);
            if (takeOut == 0)
            {
                Debug.LogWarning("当前武器所需子弹不足！");
                return;
            }

            Debug.Log($"武器换弹,当前takeout：{takeOut}");

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
            Debug.Log($"重生 wRid:{wRid}");
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
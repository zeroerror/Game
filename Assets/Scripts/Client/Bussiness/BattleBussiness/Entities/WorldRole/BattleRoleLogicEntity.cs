using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.Interfaces;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public enum RoleState
    {
        Normal,
        Move,
        RollForward,
        Hooking       //-使用爪钩中
    }

    public class BattleRoleLogicEntity : PhysicsEntity
    {

        // Connection Info
        int connId;
        public int ConnId => connId;
        public void SetConnId(int connId) => this.connId = connId;

        // ID
        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;

        // Renderer
        public BattleRoleRendererEntity roleRenderer { get; private set; }

        // Pos
        Vector3 offset;
        Vector3 shootPointPos => MoveComponent.Position + transform.forward + offset;
        public Vector3 ShootPointPos => shootPointPos.FixDecimal(4);
        public Vector3 SelfPos => transform.position;

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

            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.BattleRole);

            HealthComponent = new HealthComponent(5f);

            WeaponComponent = new WeaponComponent();

            ItemComponent = new ItemComponent();

            RoleState = RoleState.Normal;
            offset = new Vector3(0, 0.2f, 0);
        }

        public int FetchBulletsFromItemComponent()
        {
            var curWeapon = WeaponComponent.CurrentWeapon;
            return ItemComponent.TakeOutItem_Bullet(curWeapon.BulletCapacity - curWeapon.bulletNum); ;
        }

        public bool CanWeaponReload()
        {
            if (WeaponComponent.IsReloading)
            {
                return false;
            }

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

        public void Reborn(in Vector3 pos)
        {
            Debug.Log($" wRid:{idComponent.EntityId} 重生----------------------------------");

            moveComponent.SetCurPos(pos);
            moveComponent.Reset();

            HealthComponent.Reset();

            IsDead = false;
        }

        public bool IsAllowEnterNormal()
        {
            return true;
        }

    }

}
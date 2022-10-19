using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.Interfaces;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public enum RoleState
    {
        Normal,
        Rolling,
        Climbing,
        Shooting,
        Reloading,
        Healing,
        Switching,
        BeHit,
        Dead,
        Reborn
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
        public Vector3 ShootStartPos => shootPointPos.FixDecimal(4);
        public Vector3 SelfPos => transform.position;

        // == Component ==
        [SerializeField]
        MoveComponent moveComponent;
        public MoveComponent MoveComponent => moveComponent;

        [SerializeField]
        HealthComponent healthComponent;
        public HealthComponent HealthComponent => healthComponent;

        [SerializeField]
        WeaponComponent weaponComponent;
        public WeaponComponent WeaponComponent => weaponComponent;

        [SerializeField]
        RoleStateComponent stateComponent;
        public RoleStateComponent StateComponent => stateComponent;

        public ItemComponent ItemComponent { get; private set; }

        RoleInputComponent roleInputComponent;
        public RoleInputComponent InputComponent => roleInputComponent;

        public Rigidbody RB { get; private set; }


        public bool IsDead { get; private set; }

        public void Inject(BattleRoleRendererEntity roleRendererEntity)
        {
            roleRenderer = roleRendererEntity;
        }

        public void Ctor()
        {
            // == Component
            RB = transform.GetComponentInParent<Rigidbody>();
            moveComponent.Inject(RB);
            moveComponent.SetMaximumSpeed(30f);

            idComponent = new IDComponent();    // TODO: Serializable
            idComponent.SetEntityType(EntityType.BattleRole);

            roleInputComponent = new RoleInputComponent();
            ItemComponent = new ItemComponent(); // TODO: Serializable

            moveComponent.Reset();
            healthComponent.Reset();
            weaponComponent.Reset();
            stateComponent.Reset();

            offset = new Vector3(0, 0.8f, 0);
        }

        public int FetchBulletsFromItemComponent()
        {
            var curWeapon = weaponComponent.CurrentWeapon;
            return ItemComponent.TakeOutItem_Bullet(curWeapon.BulletCapacity - curWeapon.bulletNum); ;
        }

        public bool CanWeaponReload()
        {
            if (weaponComponent.IsReloading)
            {
                return false;
            }

            var curWeapon = weaponComponent.CurrentWeapon;
            if (curWeapon == null)
            {
                Debug.LogWarning("当前尚未持有武器！");
                return false;
            }

            if (weaponComponent.IsFullReloaded)
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
            Debug.Log($" ENTITYID:{idComponent.EntityId} 重生----------------------------------");

            moveComponent.SetCurPos(pos);
            moveComponent.Reset();

            healthComponent.Reset();

            IsDead = false;
        }

        public bool IsAllowEnterNormal()
        {
            return true;
        }

    }

}
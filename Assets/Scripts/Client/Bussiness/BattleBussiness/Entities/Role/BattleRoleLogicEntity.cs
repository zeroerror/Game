using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleRoleLogicEntity : PhysicsEntity
    {

        // - Connection Info
        int connId;
        public int ConnId => connId;
        public void SetConnId(int connId) => this.connId = connId;

        // - Renderer
        public BattleRoleRendererEntity roleRenderer { get; private set; }

        // - Pos
        public Vector3 SelfPos => transform.position;
        Vector3 shootPointPos => MoveComponent.Position + transform.forward + new Vector3(0, 0.8f, 0);

        [SerializeField]
        [Header("前滚翻速度")]
        float rollSpeed;
        public float RollSpeed => rollSpeed;

        bool isDead;
        public bool IsDead => isDead;

        BattleArmorEntity armor;
        public BattleArmorEntity Armor => armor;

        #region [Component]

        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;
        ItemComponent itemComponent;
        public ItemComponent ItemComponent => itemComponent;

        RoleInputComponent roleInputComponent;
        public RoleInputComponent InputComponent => roleInputComponent;

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

        #endregion

        public void Inject(BattleRoleRendererEntity roleRendererEntity)
        {
            roleRenderer = roleRendererEntity;
        }

        public void Ctor()
        {
            // == Component
            var rb = transform.GetComponentInParent<Rigidbody>();
            moveComponent.Inject(rb);
            moveComponent.SetMaximumVelocity(30f);

            idComponent = new IDComponent();    // TODO: Serializable
            idComponent.SetEntityType(EntityType.BattleRole);

            roleInputComponent = new RoleInputComponent();
            itemComponent = new ItemComponent(); // TODO: Serializable

            moveComponent.Reset();
            healthComponent.Reset();
            weaponComponent.Reset();
            stateComponent.Reset();

        }

        public void TearDown()
        {
            isDead = true;
        }

        #region [Action]

        public void Roll(Vector3 dir)
        {
            dir.Normalize();
            var addVelocity = dir * rollSpeed;
            addVelocity.y = 3f;
            moveComponent.AddExtraVelocity(addVelocity);
            Debug.Log($"前滚翻 dir {dir} rollSpeed {rollSpeed} addVelocity:{addVelocity}");
        }

        public void JumpboardSpeedUp()
        {
            var mc = moveComponent;
            var addVelocity = mc.Velocity * 4f;
            addVelocity = new Vector3(addVelocity.x, 4f, addVelocity.z);
            mc.AddExtraVelocity(addVelocity);
            DebugExtensions.LogWithColor($"跳板起飞  加速 {addVelocity} ExtraVelocity当前值: {mc.ExtraVelocity}", "#48D1CC");
        }

        public int ReloadBulletsToWeapon(WeaponEntity weaponEntity)
        {
            var num = ItemComponent.TakeOutItem_Bullet(weaponEntity.bulletType, weaponEntity.GetReloadBulletNum());
            weaponComponent.FinishReloading(num);

            return num;
        }

        public void Reborn(in Vector3 pos)
        {
            Debug.Log($" ENTITYID:{idComponent.EntityID} 重生----------------------------------");

            var mc = moveComponent;
            mc.SetPosition(pos);
            mc.Reset();

            healthComponent.Reset();

            isDead = false;
        }

        public void WearOrSwitchArmor(BattleArmorEntity val)
        {
            armor = val;
            val.IDComponent.SetLeagueId(idComponent.LeagueId);
        }

        public void DropArmor()
        {
            if (armor == null)
            {
                return;
            }

            armor = null;
        }

        #endregion

        public bool CanWeaponReload()
        {
            var wc = weaponComponent;
            if (wc.IsReloading)
            {
                return false;
            }

            var curWeapon = wc.CurrentWeapon;
            if (curWeapon == null)
            {
                Debug.LogWarning("当前尚未持有武器！");
                return false;
            }

            if (wc.IsFullReloaded)
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

    }

}
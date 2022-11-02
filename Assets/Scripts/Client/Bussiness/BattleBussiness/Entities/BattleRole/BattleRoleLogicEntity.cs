using UnityEngine;
using Game.Library;
using Game.Generic;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleRoleLogicEntity : PhysicsEntity, IPickable
    {

        #region [Component]

        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;
        ItemComponent itemComponent;
        public ItemComponent ItemComponent => itemComponent;

        RoleInputComponent roleInputComponent;
        public RoleInputComponent InputComponent => roleInputComponent;

        [SerializeField]
        LocomotionComponent locomotionComponent;
        public LocomotionComponent LocomotionComponent => locomotionComponent;

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

        // - Connection Info
        int connId;
        public int ConnId => connId;
        public void SetConnId(int connId) => this.connId = connId;

        // - Renderer
        public BattleRoleRendererEntity roleRenderer { get; private set; }

        // - Roll
        [SerializeField]
        [Header("前滚翻速度")]
        float rollSpeed;
        public float RollSpeed => rollSpeed;

        bool isDead;
        public bool IsDead => isDead;

        // - Armor
        BattleArmorEntity armor;
        public BattleArmorEntity Armor => armor;

        // - Interface
        EntityType IPickable.EntityType => idComponent.EntityType;
        int IPickable.EntityID => idComponent.EntityID;

        public void Inject(BattleRoleRendererEntity roleRendererEntity)
        {
            roleRenderer = roleRendererEntity;
        }

        public void Ctor()
        {
            // == Component
            var rb = transform.GetComponentInParent<Rigidbody>();
            locomotionComponent.Inject(rb);
            locomotionComponent.SetMaximumVelocity(30f);

            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.BattleRole);

            roleInputComponent = new RoleInputComponent();

            itemComponent = new ItemComponent();

            locomotionComponent.Reset();
            healthComponent.Reset();
            weaponComponent.Reset();
            stateComponent.Reset();

        }

        public void TearDown()
        {
            isDead = true;
        }

        #region [Action]

        public bool TryRoll(Vector3 dir)
        {
            if (dir == Vector3.zero)
            {
                return false;
            }

            var mc = locomotionComponent;
            if (!mc.IsGrounded)
            {
                return false;
            }

            dir.Normalize();
            var addVelocity = dir * rollSpeed;
            addVelocity.y = 3f;
            mc.AddExtraVelocity(addVelocity);
            stateComponent.EnterRolling(30);

            Debug.Log($"前滚翻 dir {dir} rollSpeed {rollSpeed} addVelocity:{addVelocity}");
            return true;
        }

        public void JumpboardSpeedUp()
        {
            var mc = locomotionComponent;
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

            var mc = locomotionComponent;
            mc.SetPosition(pos);
            mc.Reset();

            healthComponent.Reset();

            isDead = false;
        }

        public void EvolveFrom(EvolveTM evolveTM)
        {
            // - Health
            var hc = healthComponent;
            hc.AddCurHealth(evolveTM.addHealth);

            // - Locomotion
            var lc = LocomotionComponent;
            lc.AddBasicMoveSpeed(evolveTM.addSpeed);

            // - WeaponComponent
            var wc = weaponComponent;
            wc.AddDamageCoefficient(evolveTM.addDamageCoefficient);
        }

        // - Armor
        public void WearArmro(BattleArmorEntity v)
        {
            if (HasArmor())
            {
                return;
            }

            var idc = v.IDComponent;
            idc.SetLeagueId(idComponent.LeagueId);
            armor = v;
        }

        public float TryReceiveDamage(float damage)
        {
            float armorReceiveDamage = 0;

            // - Armor
            if (HasArmor())
            {
                armorReceiveDamage = armor.TryRecieveDamage(damage);
                if (armorReceiveDamage == damage)
                {
                    Debug.Log($"Role Receive Damage: armorReceiveDamage {armorReceiveDamage}");
                    return armorReceiveDamage;
                }
            }

            // - Flesh
            var fleshReceiveDamage = healthComponent.TryReiveDamage(damage - armorReceiveDamage);
            Debug.Log($"Role Receive Damage: armorReceiveDamage {armorReceiveDamage} fleshReceiveDamage {fleshReceiveDamage}");

            var totalReceiveDamage = armorReceiveDamage + fleshReceiveDamage;
            return totalReceiveDamage;
        }

        #endregion

        public bool HasArmor()
        {
            return armor != null;
        }

        public bool CanWeaponShoot()
        {
            var wc = weaponComponent;
            var curWeapon = wc.CurrentWeapon;

            if (curWeapon == null)
            {
                Debug.LogWarning("当前武器为空，无法射击");
                return false;
            }

            if (wc.IsReloading)
            {
                Debug.LogWarning("换弹中，无法射击");
                return false;
            }

            var sc = stateComponent;
            var shootingMod = sc.ShootingMod;
            if (sc.RoleState == RoleState.Shooting && shootingMod.maintainFrame > shootingMod.breakFrame)
            {
                Debug.LogWarning("射击CD未结束");
                return false;
            }

            return true;
        }

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

            if (wc.IsFullReloaded())
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
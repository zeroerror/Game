using UnityEngine;
using Game.Library;
using Game.Generic;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleRoleLogicEntity : PhysicsEntity
    {

        #region [Component]

        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;
        public void SetLeagueID(int v) => idComponent.SetLeagueID(v);
        public void SetEntityID(int v) => idComponent.SetEntityID(v);

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
        public int ConnID => connId;
        public void SetConnID(int connId) => this.connId = connId;

        // - Renderer
        public BattleRoleRendererEntity roleRenderer { get; private set; }

        bool isDead;
        public bool IsDead => isDead;

        // - Armor
        BattleArmorEntity armor;
        public BattleArmorEntity Armor => armor;

        public void Inject(BattleRoleRendererEntity roleRendererEntity)
        {
            roleRenderer = roleRendererEntity;
        }

        public void Ctor()
        {
            // == Component
            var rb = transform.GetComponentInParent<Rigidbody>();
            locomotionComponent.Inject(rb);
            locomotionComponent.Ctor();

            healthComponent.Ctor();

            weaponComponent.Ctor();

            stateComponent.Ctor();

            roleInputComponent = new RoleInputComponent();

            itemComponent = new ItemComponent();

            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.BattleRole);
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

            var lc = locomotionComponent;
            if (!lc.IsGrounded)
            {
                return false;
            }

            var rollSpeed = lc.BasicMoveSpeed * 3;
            var maxVelocity = lc.MaxVelocity;
            rollSpeed = rollSpeed > maxVelocity ? maxVelocity : rollSpeed;

            dir.Normalize();
            var addVelocity = dir * rollSpeed;
            addVelocity.y = 5f;
            lc.AddExtraVelocity(addVelocity);
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
            var num = ItemComponent.TakeOutItem_Bullet(weaponEntity.BulletType, weaponEntity.GetReloadBulletNum());
            weaponComponent.FinishReloading(num);

            return num;
        }

        public void Reborn(in Vector3 pos)
        {
            Debug.Log($" ENTITYID:{idComponent.EntityID} 重生----------------------------------");

            // - Locomotion
            var mc = locomotionComponent;
            mc.SetPosition(pos);
            mc.Reset();

            // - Health
            healthComponent.Reset();

            // - Weapon
            weaponComponent.Reset();

            // - Item
            itemComponent.Reset();

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
            var allWeapons = wc.AllWeapons;
            for (int i = 0; i < allWeapons.Length; i++)
            {
                var weapon = allWeapons[i];
                if (weapon != null)
                {
                    weapon.AddDamageCoefficient(evolveTM.addDamageCoefficient);
                }
            }
        }

        // - Armor
        public bool TryWearArmro(BattleArmorEntity v)
        {
            if (HasArmor())
            {
                return false;
            }

            SetLeagueID(idComponent.LeagueId);
            armor = v;
            return true;
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
                    Debug.Log($"damage {damage} armorReceiveDamage {armorReceiveDamage}");
                    return armorReceiveDamage;
                }
            }

            // - Flesh
            var fleshReceiveDamage = healthComponent.TryReiveDamage(damage - armorReceiveDamage);
            Debug.Log($"damage {damage}: armorReceiveDamage {armorReceiveDamage} fleshReceiveDamage {fleshReceiveDamage}");

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
            var curWeapon = wc.CurWeapon;

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

            var curWeapon = wc.CurWeapon;
            if (curWeapon == null)
            {
                Debug.LogWarning("当前尚未持有武器!");
                return false;
            }
            if (wc.IsFullReloaded())
            {
                Debug.LogWarning("当前武器已经装满子弹!");
                return false;
            }
            if (wc.IsReloading)
            {
                Debug.LogWarning("当前正在换弹!");
                return false;
            }
            if (!ItemComponent.HasItem_Bullet(curWeapon.BulletType, 1))
            {
                Debug.LogWarning($"当前子弹类型 {curWeapon.BulletType.ToString()} 1颗都没了!");
                return false;
            }

            return true;
        }

    }

}
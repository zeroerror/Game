using System.Collections.Generic;
using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BulletLogicEntity : PhysicsEntity
    {

        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;
        public void SetLeagueID(int v) => idComponent.SetLeagueID(v);
        public void SetEntityID(int v) => idComponent.SetEntityID(v);

        // Master
        int weaponID;
        public int WeaponID => weaponID;
        public void SetWeaponID(int v) => this.weaponID = v;

        [SerializeField] BulletType bulletType = BulletType.DefaultBullet;
        public BulletType BulletType => bulletType;
        public void SetBulletType(BulletType bulletType) => this.bulletType = bulletType;

        [SerializeField] protected LocomotionComponent locomotionComponent;
        public LocomotionComponent LocomotionComponent => locomotionComponent;


        [SerializeField] protected HitPowerModel hitPowerModel;
        public HitPowerModel HitPowerModel => hitPowerModel;

        [SerializeField][Header("单位/秒")] float lifeTime;
        public float LifeTime => lifeTime;
        public void SetLifeTime(float lifeTime) => this.lifeTime = lifeTime;
        public void ReduceLifeTime(float time) => this.lifeTime -= time;

        float existTime;
        public float ExistTime => existTime;
        public void AddExistTime(float time) => existTime += time;

        public void Ctor()
        {
            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.Bullet);

            locomotionComponent.Inject(transform.GetComponent<Rigidbody>());
            Init();
        }

        protected virtual void Init() { }

        public virtual void TearDown()
        {
            Debug.Log($"摧毁子弹Logic --- {idComponent.EntityID}");
            Destroy(gameObject);
            Destroy(this);
        }

        public void SetPosition(Vector3 pos)
        {
            locomotionComponent.SetPosition(pos);
            transform.position = pos;
        }

        public void FaceTo(Vector3 forward)
        {
            locomotionComponent.FaceTo(forward);
            transform.rotation = locomotionComponent.RB.rotation;
        }

        public float GetDamageByCoefficient(float coefficient)
        {
            var realDamage = hitPowerModel.damage * coefficient;
            return realDamage;
        }

    }

}
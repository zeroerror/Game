using System.Collections.Generic;
using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BulletEntity : PhysicsEntity
    {
        // ID Info
        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;

        // Master Info
        int masterEntityId;
        public int MasterEntityId => masterEntityId;
        public void SetMasterEntityId(int masterEntityId) => this.masterEntityId = masterEntityId;

        // Bullet Info
        [SerializeField]
        BulletType bulletType = BulletType.DefaultBullet;
        public BulletType BulletType => bulletType;
        public void SetBulletType(BulletType bulletType) => this.bulletType = bulletType;

        [SerializeField]
        protected LocomotionComponent locomotionComponent;
        public LocomotionComponent LocomotionComponent => locomotionComponent;

        [SerializeField]
        protected HitPowerModel hitPowerModel;
        public HitPowerModel HitPowerModel => hitPowerModel;

        // Life 
        [SerializeField]
        float lifeTime;
        public float LifeTime => lifeTime;
        public void SetLifeTime(float lifeTime) => this.lifeTime = lifeTime;
        public void ReduceLifeTime(float time) => this.lifeTime -= time;

        // Damage Coefficient
        float damageCoefficient;

        float existTime;
        public float ExistTime => existTime;
        public void AddExistTime(float time) => existTime += time;

        public void Ctor()
        {
            damageCoefficient = 1f;

            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.Bullet);

            locomotionComponent.Inject(transform.GetComponent<Rigidbody>());
            Init();
        }

        protected virtual void Init() { }

        public virtual void TearDown()
        {
            Debug.Log($"摧毁子弹  {bulletType.ToString()} {idComponent.EntityID}");
            Destroy(gameObject);
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

        public void SetDamageByCoefficient(float coefficient)
        {
            hitPowerModel.damage *= coefficient;
        }

    }

}
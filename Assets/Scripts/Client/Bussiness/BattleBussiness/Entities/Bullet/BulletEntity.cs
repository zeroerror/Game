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
        protected MoveComponent moveComponent;
        public MoveComponent MoveComponent => moveComponent;

        [SerializeField]
        protected HitPowerModel hitPowerModel;
        public HitPowerModel HitPowerModel => hitPowerModel;

        // Life 

        [SerializeField]
        float lifeTime;
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

            moveComponent.Inject(transform.GetComponent<Rigidbody>());
            Init();
        }

        protected virtual void Init()
        {

        }

        public virtual void TearDown()
        {
            Debug.Log($"摧毁子弹  {bulletType.ToString()} {idComponent.EntityId}");
            Destroy(gameObject);
        }

    }

}
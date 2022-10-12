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
        byte masterId;
        public byte MasterId => masterId;
        public void SetMasterId(byte masterId) => this.masterId = masterId;

        // Bullet Info
        [SerializeField]
        BulletType bulletType = BulletType.DefaultBullet;
        public BulletType BulletType => bulletType;
        public void SetBulletType(BulletType bulletType) => this.bulletType = bulletType;

        [SerializeField]
        protected MoveComponent moveComponent;
        public MoveComponent MoveComponent => moveComponent;

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
            Destroy(gameObject);
        }

    }

}
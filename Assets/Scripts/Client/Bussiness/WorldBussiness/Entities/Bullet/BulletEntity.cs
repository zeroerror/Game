using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public enum BulletType
    {
        Default,
        Grenade,
        Hooker
    }

    public class BulletEntity : PhysicsEntity
    {
        // Master Info
        byte wRid;
        public byte WRid => wRid;
        public void SetWRid(byte wRid) => this.wRid = wRid;

        // Bullet Info
        BulletType bulletType = BulletType.Default;
        public BulletType BulletType => bulletType;
        public void SetBulletType(BulletType bulletType) => this.bulletType = bulletType;

        ushort bulletId;
        public ushort BulletId => bulletId;
        public void SetBulletId(ushort bulletId) => this.bulletId = bulletId;

        protected MoveComponent moveComponent;
        public MoveComponent MoveComponent => moveComponent;


        // Life 
        float lifeTime;
        public float LifeTime => lifeTime;
        public void SetLifeTime(float lifeTime) => this.lifeTime = lifeTime;
        public void ReduceLifeTime(float time) => this.lifeTime -= time;

        float existTime;
        public float ExistTime => existTime;
        public void AddExistTime(float time) => existTime += time;

        // Physics Queue
        public Queue<WorldRoleEntity> HitRoleQueue { get; private set; }
        public Queue<GameObject> HitFieldQueue { get; private set; }

        public void Ctor()
        {
            moveComponent = new MoveComponent(transform.GetComponent<Rigidbody>());
            moveComponent.SetSpeed(30f);
            moveComponent.SetGravity(0);
            moveComponent.isPersistentMove = true;

            lifeTime = 5f;
            HitRoleQueue = new Queue<WorldRoleEntity>();
            HitFieldQueue = new Queue<GameObject>();
            Init();
        }

        protected virtual void Init()
        {

        }

        public virtual void TearDown()
        {
            Destroy(gameObject);
        }

        // Unity Physics 

        public virtual void EnterTrigger(Collider collision) { }
        public virtual void ExitTrigger(Collider collision) { }
        public virtual void EnterCollision(Collision collision) { }
        public virtual void ExitCollision(Collision collision) { }

    }

}
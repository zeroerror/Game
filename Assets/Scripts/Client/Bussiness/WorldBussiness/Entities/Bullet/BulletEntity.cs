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

    public class BulletEntity : MonoBehaviour
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

        Queue<WorldRoleEntity> hitRoleQueue;

        // Life 
        float lifeTime;
        public float LifeTime => lifeTime;
        public void SetLifeTime(float lifeTime) => this.lifeTime = lifeTime;
        public void ReduceLifeTime(float time) => this.lifeTime -= time;

        float existTime;
        public float ExistTime => existTime;
        public void AddExistTime(float time) => existTime += time;

        // Physics Queue
        public bool TryDequeue(out WorldRoleEntity roleEntity) => hitRoleQueue.TryDequeue(out roleEntity);

        public void Awake()
        {
            moveComponent = new MoveComponent(transform.GetComponent<Rigidbody>());
            moveComponent.SetSpeed(30f);
            moveComponent.SetGravity(0);
            moveComponent.isPersistentMove = true;

            lifeTime = 5f;
            hitRoleQueue = new Queue<WorldRoleEntity>();
            Init();
        }

        protected virtual void Init()
        {

        }

        public virtual void TearDown()
        {

        }

        // Unity Physics 

        public virtual void EnterTrigger(Collider collision) { }
        public virtual void ExitTrigger(Collider collision) { }
        public virtual void EnterCollision(Collision collision) { }
        public virtual void ExitCollision(Collision collision) { }

        void OnTriggerEnter(Collider collision)
        {
            EnterTrigger(collision);
        }

        void OnTriggerExit(Collider collision)
        {
            ExitTrigger(collision);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Field"))
            {
                MoveComponent.EnterGround();
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                MoveComponent.HitWall();
            }
            EnterCollision(collision);
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Field"))
            {
                MoveComponent.LeaveGround();
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                MoveComponent.LeaveWall();
            }
            ExitCollision(collision);
        }

    }

}
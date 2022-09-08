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
        Queue<WorldRoleEntity> hitRoleQueue;
        public bool TryDequeueHitRole(out WorldRoleEntity roleEntity) => hitRoleQueue.TryDequeue(out roleEntity);
        Queue<GameObject> hitWallQueue;
        public bool TryDequeueHitWall(out GameObject wall) => hitWallQueue.TryDequeue(out wall);

        public void Awake()
        {
            moveComponent = new MoveComponent(transform.GetComponent<Rigidbody>());
            moveComponent.SetSpeed(30f);
            moveComponent.SetGravity(0);
            moveComponent.isPersistentMove = true;

            lifeTime = 5f;
            hitRoleQueue = new Queue<WorldRoleEntity>();
            hitWallQueue = new Queue<GameObject>();
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

        void OnTriggerEnter(Collider collider)
        {
            GameObject colliderGo = collider.gameObject;
            var layer = colliderGo.layer;

            if (layer == LayerMask.NameToLayer("Role"))
            {
                hitRoleQueue.Enqueue(colliderGo.transform.GetComponent<WorldRoleEntity>());
            }
            if (layer == LayerMask.NameToLayer("Field"))
            {
                MoveComponent.EnterField();
            }
            if (layer == LayerMask.NameToLayer("Wall"))
            {
                MoveComponent.EnterWall();
                hitWallQueue.Enqueue(colliderGo);
            }
            EnterTrigger(collider);
        }

        void OnTriggerExit(Collider collider)
        {
            GameObject colliderGo = collider.gameObject;
            var layer = colliderGo.layer;

            if (layer == LayerMask.NameToLayer("Role"))
            {
            }
            if (layer == LayerMask.NameToLayer("Field"))
            {
                MoveComponent.LeaveGround();
            }
            if (layer == LayerMask.NameToLayer("Wall"))
            {
                MoveComponent.LeaveWall();
            }
            ExitTrigger(collider);
        }

        void OnCollisionEnter(Collision collision)
        {
            GameObject collisionGo = collision.gameObject;
            if (collisionGo.layer == LayerMask.NameToLayer("Role"))
            {
                hitRoleQueue.Enqueue(collisionGo.transform.GetComponent<WorldRoleEntity>());
            }
            if (collisionGo.layer == LayerMask.NameToLayer("Field"))
            {
                MoveComponent.EnterField();
            }
            if (collisionGo.layer == LayerMask.NameToLayer("Wall"))
            {
                MoveComponent.EnterWall();
                hitWallQueue.Enqueue(collisionGo);
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
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public enum BulletType
    {
        Default,
        Grenade
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

        public MoveComponent MoveComponent { get; private set; }

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
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>(), 50f, 0f);
            MoveComponent.isPersistentMove = true;

            hitRoleQueue = new Queue<WorldRoleEntity>();
        }

        public virtual void TearDown()
        {

        }

        void OnTriggerEnter(Collider collision)
        {
            var go = collision.gameObject;
            var layerName = LayerMask.LayerToName(go.layer);
            if (layerName == "Player")
            {
                hitRoleQueue.Enqueue(go.GetComponent<WorldRoleEntity>());
            }
        }

    }

}
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class BulletEntity : MonoBehaviour
    {
        // Master Info
        byte wRid;
        public byte WRid => wRid;
        public void SetWRid(byte wRid) => this.wRid = wRid;

        // Bullet Info
        public ushort bulletId;

        public void SetBulletId(ushort bulletId) => this.bulletId = bulletId;
        public bool IsHit { get; private set; }
        public void SetHit() => IsHit = true;

        public MoveComponent MoveComponent { get; private set; }

        public void Awake()
        {
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>());
        }

    }

}
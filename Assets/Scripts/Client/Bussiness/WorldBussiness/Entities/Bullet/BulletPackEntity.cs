using Game.Client.Bussiness.WorldBussiness.Generic;
using Game.Client.Bussiness.WorldBussiness.Interface;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class BulletPackEntity : MonoBehaviour, IPickable
    {

        ItemType itemType;
        public ItemType ItemType => itemType;

        // 子弹包具体的子弹类型
        [SerializeField]
        public BulletType bulletType;

        [SerializeField]
        public int bulletNum;

        // Master Info
        byte masterId;
        public byte MasterId => masterId;
        public void SetMasterId(byte masterId) => this.masterId = masterId;

        ushort entityId;
        public ushort EntityId => entityId;
        public void SetEntityId(ushort entityId) => this.entityId = entityId;

        public void Ctor()
        {
            itemType = ItemType.BulletPack;
        }

        public void TearDown()
        {
            Destroy(gameObject);
        }

    }

}
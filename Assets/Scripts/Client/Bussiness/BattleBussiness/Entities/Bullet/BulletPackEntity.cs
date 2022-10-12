using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
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
        int masterId;
        public int MasterId => masterId;
        public void SetMasterId(int masterId) => this.masterId = masterId;

        int entityId;
        public int EntityId => entityId;
        public void SetEntityId(int entityId) => this.entityId = entityId;

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
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BulletItemEntity : MonoBehaviour, IPickable
    {

        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;

        [SerializeField]
        public BulletType bulletType;

        [SerializeField]
        public int bulletNum;

        // Master Info
        int masterId;
        public int MasterId => masterId;

        EntityType IPickable.EntityType => idComponent.EntityType;
        int IPickable.EntityID => idComponent.EntityID;

        public void SetMasterId(int masterId) => this.masterId = masterId;

        public void Ctor()
        {
            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.BulletItem);
            idComponent.SetSubType((byte)bulletType);
        }

        public void TearDown()
        {
            Destroy(gameObject);
        }

    }

}
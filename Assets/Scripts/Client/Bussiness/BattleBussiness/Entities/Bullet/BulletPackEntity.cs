using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BulletPackEntity : MonoBehaviour, IPickable
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
        public void SetMasterId(int masterId) => this.masterId = masterId;

        // - Interface
        EntityType IPickable.EntityType => idComponent.EntityType;
        int IPickable.EntityID => idComponent.EntityID;
        int IPickable.MasterId => masterId;

        public void Ctor()
        {
            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.BulletPack);
        }

        public void TearDown()
        {
            Destroy(gameObject);
        }

    }

}
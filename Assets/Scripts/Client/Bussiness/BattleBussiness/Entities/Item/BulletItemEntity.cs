using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BulletItemEntity : MonoBehaviour, IPickable
    {

        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;
        public void SetLeagueID(int v) => idComponent.SetLeagueID(v);
        public void SetEntityID(int v) => idComponent.SetEntityID(v);

        [SerializeField]
        BulletType bulletType;
        public BulletType BulletType => bulletType;

        [SerializeField]
        public int bulletNum;

        // Master Info
        int masterID;
        public int MasterID => masterID;

        EntityType IPickable.EntityType => idComponent.EntityType;
        int IPickable.EntityID => idComponent.EntityID;

        public void SetMasterId(int masterId) => this.masterID = masterId;

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
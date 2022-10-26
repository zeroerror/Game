using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BulletPackEntity : MonoBehaviour
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
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class WeaponItemEntity : PhysicsEntity, IPickable
    {

        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;

        [SerializeField]
        WeaponType weaponType;
        public WeaponType WeaponType => weaponType;

        EntityType IPickable.EntityType => idComponent.EntityType;
        int IPickable.EntityID => idComponent.EntityID;

        public void Ctor()
        {
            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.WeaponItem);
            idComponent.SetSubType((byte)weaponType);
        }

    }

}
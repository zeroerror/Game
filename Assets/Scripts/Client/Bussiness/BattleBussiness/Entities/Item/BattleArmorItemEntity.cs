using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleArmorItemEntity : MonoBehaviour, IPickable
    {
        // == Component
        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;

        [SerializeField]
        ArmorType armorType;
        public ArmorType ArmorType => armorType;

        EntityType IPickable.EntityType => idComponent.EntityType;
        int IPickable.EntityID => idComponent.EntityID;

        Rigidbody rb;

        public void Ctor()
        {
            rb = GetComponentInChildren<Rigidbody>();

            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.ArmorItem);
            idComponent.SetSubType((byte)armorType);
        }

    }

}
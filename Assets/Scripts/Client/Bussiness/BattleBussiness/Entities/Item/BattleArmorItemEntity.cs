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
        GameObject armorPrefab;
        public string GetArmorPrefabName() => armorPrefab.name;

        EntityType IPickable.EntityType => idComponent.EntityType;
        int IPickable.EntityID => idComponent.EntityID;

        Rigidbody rb;

        public void Ctor()
        {
            rb = GetComponentInChildren<Rigidbody>();

            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.ArmorItem);
        }

    }

}
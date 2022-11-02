using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;
using Game.Library;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleArmorEvolveItemEntity : MonoBehaviour, IPickable
    {

        // == Component
        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;

        public EvolveTM evolveTM;

        EntityType IPickable.EntityType => idComponent.EntityType;

        int IPickable.EntityID => idComponent.EntityID;

        public void Ctor()
        {
            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.ArmorEvolveItem);
        }

    }

}
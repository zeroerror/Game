using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;
using Game.Library;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleEvolveItemEntity : MonoBehaviour, IPickable
    {

        // == Component
        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;
        public void SetLeagueID(int v) => idComponent.SetLeagueID(v);
        public void SetEntityID(int v) => idComponent.SetEntityID(v);
        
        public EntityType evolveEntityType;
        public EvolveTM evolveTM;

        EntityType IPickable.EntityType => idComponent.EntityType;

        int IPickable.EntityID => idComponent.EntityID;

        public void Ctor()
        {
            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.EvolveItem);
        }

    }

}
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Library;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleAirdropEntity : MonoBehaviour
    {

        int entityID;
        public int EntityID => entityID;
        public void SetEntityID(int v) => entityID = v;

        public Rigidbody RB { get; private set; }

        [SerializeField] EntityType entityType; // For Spawn
        public EntityType EntityType => entityType;

        [SerializeField] byte subType; // For Spawn
        public byte SubType => subType;

        // == Component
        [SerializeField] LocomotionComponent locomotionComponent;
        public LocomotionComponent LocomotionComponent => locomotionComponent;

        public void Ctor()
        {
            RB = transform.GetComponent<Rigidbody>();
            locomotionComponent.Inject(RB);
        }

        public void Reset()
        {
            locomotionComponent.Reset();
        }

    }

}
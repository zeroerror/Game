using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleAirdropEntity : PhysicsEntity
    {

        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;

        public Rigidbody RB { get; private set; }

        [SerializeField] AirdropType airdropType; // For Spawn
        public AirdropType AirdropType => airdropType; // For Spawn

        [SerializeField] EntityType spawnEntityType; // For Spawn
        public EntityType SpawnEntityType => spawnEntityType;

        [SerializeField] byte spawnSubType; // For Spawn
        public byte SpawnSubType => spawnSubType;

        // == Component
        [SerializeField] LocomotionComponent locomotionComponent;
        public LocomotionComponent LocomotionComponent => locomotionComponent;

        [SerializeField] HealthComponent healthComponent;
        public HealthComponent HealthComponent => healthComponent;

        public void Ctor()
        {
            Ctor_Component();
        }

        void Ctor_Component()
        {
            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.Aridrop);
            idComponent.SetSubType((byte)airdropType);

            RB = transform.GetComponent<Rigidbody>();
            locomotionComponent.Inject(RB);
            locomotionComponent.Ctor();
            healthComponent.Ctor();
        }

        public void Reset()
        {
            locomotionComponent.Reset();
        }

        public void TearDown()
        {
            GameObject.Destroy(gameObject);
            GameObject.Destroy(this);
        }

    }

}
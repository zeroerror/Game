using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleArmorEntity : PhysicsEntity
    {
        // == Component
        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;

        [SerializeField]
        ArmorType armorType;
        public ArmorType ArmorType => armorType;

        [SerializeField]
        int health;
        public int Health => health;

        int curHealth;
        public int CurHealth => curHealth;

        Rigidbody rb;

        public void Ctor()
        {
            rb = GetComponentInChildren<Rigidbody>();

            idComponent = new IDComponent();
            idComponent.SetSubType((byte)armorType);
        }

        public void RecieveDamage(int damage)
        {
            curHealth -= damage;
            if (curHealth < 0)
            {
                curHealth = 0;
            }
        }

        public void Reset()
        {
            idComponent.SetLeagueId(-1);
            curHealth = health;
        }

    }

}
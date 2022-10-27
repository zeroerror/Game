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

        public int TryRecieveDamage(int damage)
        {
            int realDamage = 0;
            if (curHealth >= damage)
            {
                realDamage = damage;
                curHealth -= realDamage;
                return realDamage;
            }

            realDamage = curHealth;
            curHealth = 0;

            return realDamage;
        }

        public void Reset()
        {
            idComponent.SetLeagueId(-1);
            curHealth = health;
        }

    }

}
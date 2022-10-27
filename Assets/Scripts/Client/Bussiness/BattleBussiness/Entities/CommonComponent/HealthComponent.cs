using UnityEngine;
using Game.Generic;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public class HealthComponent
    {

        [SerializeField]
        int maxHealth;
        public int MaxHealth => maxHealth;

        [SerializeField]
        int health;
        public int Health => health;

        public bool CheckIsDead() => health <= 0;

        public int TryReiveDamage(int damage)
        {
            int realDamage = 0;
            if (health >= damage)
            {
                realDamage = damage;
                health -= realDamage;
                return realDamage;
            }

            realDamage = health;
            health = 0;

            return realDamage;
        }

        public void Reset()
        {
            health = maxHealth;
        }

    }

}
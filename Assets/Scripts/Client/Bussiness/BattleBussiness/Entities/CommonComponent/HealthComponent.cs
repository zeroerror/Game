using UnityEngine;
using Game.Generic;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public class HealthComponent
    {

        [SerializeField]
        float maxHealth;
        public float MaxHealth => maxHealth;

        [SerializeField]
        float health;
        public float Health => health;

        public bool CheckIsDead() => health <= 0;

        public void HurtByDamage(int damage)
        {
            Debug.Log($"受到伤害: {damage}  ->  health {health - damage}");
            health -= damage;
        }

        public void Reset()
        {
            health = maxHealth;
        }

    }

}
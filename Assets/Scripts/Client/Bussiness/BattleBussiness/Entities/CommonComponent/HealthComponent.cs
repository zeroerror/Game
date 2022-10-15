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

        public bool IsDead() => health <= 0;

        public void HurtByDamage(float damage)
        {
            if (IsDead()) return;

            Debug.Log($"health:{health} -> {health - damage}");
            health -= damage;
        }

        public void HurtByDamage(int damage)
        {
            // TODO: 改成子弹对应的伤害
            if (IsDead()) return;

            Debug.Log($"受到伤害: {damage}  ->  health {health - damage}");
            health -= damage;
        }

        public void Reset()
        {
            health = maxHealth;
        }

    }

}
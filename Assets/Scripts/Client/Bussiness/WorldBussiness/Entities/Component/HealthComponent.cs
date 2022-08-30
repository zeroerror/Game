using UnityEngine;
using Game.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class HealthComponent
    {

        float maxHealth;
        public float MaxHealth => maxHealth;
        
        float health;
        public float Health => health;

        public bool IsDead => health <= 0;

        public HealthComponent(float health)
        {
            this.maxHealth = health;
            this.health = health;
        }

        public void HurtByDamage(float damage)
        {
            if (IsDead) return;

            Debug.Log($"health:{health} -> {health - damage}");
            health -= damage;
        }

        public void HurtByBullet(BulletEntity bulletEntity)
        {
            // TODO: 改成子弹对应的伤害
            if (IsDead) return;

            Debug.Log($"health:{health} -> {health - 5}");
            health -= 5;
        }

        public void Reset()
        {
            health = maxHealth;
        }

    }

}
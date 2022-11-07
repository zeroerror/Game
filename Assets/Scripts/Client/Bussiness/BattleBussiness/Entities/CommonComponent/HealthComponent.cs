using UnityEngine;
using Game.Generic;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public class HealthComponent
    {

        [SerializeField] int maxHealth;
        public int MaxHealth => maxHealth;

        float curHealth;
        public float CurHealth => curHealth;
        public void SetCurHealth(float v) => curHealth = v > maxHealth ? maxHealth : v;

        public void Ctor()
        {
            curHealth = maxHealth;
        }

        public void Reset()
        {
            curHealth = maxHealth;
        }


        public bool CheckIsDead() => curHealth <= 0;

        public float TryReiveDamage(float damage)
        {
            float realDamage = 0;
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

        public void AddCurHealth(float v)
        {
            curHealth += v;
            curHealth = curHealth > maxHealth ? maxHealth : curHealth;
        }

    }

}
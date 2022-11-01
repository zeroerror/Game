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

        int curHealth;
        public int CurHealth => curHealth;
        public int SetCurHealth(int v) => curHealth = v;

        public bool CheckIsDead() => curHealth <= 0;

        public int TryReiveDamage(int damage)
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
            curHealth = maxHealth;
        }

    }

}
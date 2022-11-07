using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Library;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleArmorEntity : MonoBehaviour
    {
        // == Component
        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;
        public void SetLeagueID(int v) => idComponent.SetLeagueID(v);
        public void SetEntityID(int v) => idComponent.SetEntityID(v);

        [SerializeField]
        int maxHealth;
        public int MaxHealth => maxHealth;

        float curHealth;
        public float CurHealth => curHealth;
        public void SetCurHealth(float v) => curHealth = v;

        Rigidbody rb;

        public void Ctor()
        {
            rb = GetComponentInChildren<Rigidbody>();

            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.Armor);

            curHealth = maxHealth;
        }

        public void Reset()
        {
            SetLeagueID(-1);
            curHealth = maxHealth;
        }

        public void EvolveFrom(in EvolveTM evolveTM)
        {
            var addHealth = evolveTM.addHealth;
            var addSpeed = evolveTM.addSpeed;
            var addDamageCoefficient = evolveTM.addDamageCoefficient;
            maxHealth += addHealth;
            curHealth += addHealth;
            curHealth = curHealth > maxHealth ? maxHealth : curHealth;
            Debug.Log($"进化护甲 addHealth {addHealth} addSpeed {addSpeed} addDamageCoefficient {addDamageCoefficient} ");
        }

        public float TryRecieveDamage(float damage)
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

    }

}
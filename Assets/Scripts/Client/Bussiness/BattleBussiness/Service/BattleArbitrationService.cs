using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness
{

    public enum AttackTag : byte
    {
        None,
        Enemy,      // - 敌军
        Ally,       // - 友军
    }

    public class BattleArbitrationService
    {

        Dictionary<long, List<HitModel>> all;

        BattleFacades battleFacades;

        public BattleArbitrationService()
        {
            all = new Dictionary<long, List<HitModel>>();
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public bool IsHitSuccess(IDComponent attacker, IDComponent victim, in HitPowerModel hitPowerModel)
        {
            // - 不可多次伤害
            if (HasHitRecord(attacker, victim, hitPowerModel) && !hitPowerModel.canHitRepeatly)
            {
                Debug.LogWarning($"不可多次受击 ");
                return false;
            }

            // - 不是正确的攻击对象
            if (!IsRightTarget(attacker, victim, hitPowerModel))
            {
                Debug.LogWarning($"不是正确的受击对象 ");
                return false;
            }

            // - 不可伤害(无敌、死亡)
            if (!CanDamage(victim))
            {
                Debug.LogWarning($"不可伤害(无敌、死亡)");
                return false;
            }

            return true;
        }

        public void AddHitRecord(IDComponent attackerIDC, IDComponent victimIDC, float damage, bool hasCausedDeath)
        {
            HitModel hitModel = new HitModel();
            hitModel.attackerIDC = attackerIDC;
            hitModel.victimIDC = victimIDC;
            hitModel.damage = damage;
            hitModel.hasCausedDeath = hasCausedDeath;
            Debug.Log($"ADD HIT RECORD: damage {damage} hasCausedDeath {hasCausedDeath}");

            var key = GetKey(attackerIDC, victimIDC);

            if (all.TryGetValue(key, out var list))
            {
                list.Add(hitModel);
                return;
            }

            list = new List<HitModel>();
            list.Add(hitModel);
            all[key] = list;
        }

        public void GetAtkerTotalKillAndCauseDamage(EntityType entityType, int entityID, out int totalKill, out float totalDamage)
        {
            float damage = 0;
            int kill = 0;

            int key1 = 0;
            key1 |= (int)entityType << 16;
            key1 |= (int)entityID;

            foreach (var obj in all)
            {
                var key2 = (int)(obj.Key >> 32);
                if (key1 == key2)
                {
                    var list = obj.Value;
                    list.ForEach((hitModel) =>
                    {
                        damage += hitModel.damage;
                        if (hitModel.hasCausedDeath)
                        {
                            kill++;
                        }
                    });
                }
            }

            totalKill = kill;
            totalDamage = damage;
        }

        bool CanDamage(IDComponent victim)
        {
            if (victim.EntityType == EntityType.BattleRole)
            {
                var role = battleFacades.Repo.RoleLogicRepo.Get(victim.EntityID);
                var stateComponent = role.StateComponent;
                return stateComponent.RoleState != RoleState.Reborning && stateComponent.RoleState != RoleState.Dying;
            }

            return false;
        }

        bool IsRightTarget(IDComponent attacker, IDComponent victim, in HitPowerModel hitPowerModel)
        {

            AttackTag attackTag = hitPowerModel.attackTag;

            if (attackTag == AttackTag.None)
            {
                return false;
            }

            if (attackTag == AttackTag.Enemy)
            {
                return IsOpposite(attacker, victim);
            }

            if (attackTag == AttackTag.Ally)
            {
                return !IsOpposite(attacker, victim);
            }

            Debug.LogWarning("未处理的情况");
            return false;
        }

        bool IsOpposite(IDComponent attacker, IDComponent victim)
        {
            if (attacker.LeagueId != victim.LeagueId)
            {
                return true;
            }

            return false;
        }

        bool HasHitRecord(IDComponent attacker, IDComponent victim, in HitPowerModel hitPowerModel)
        {
            long key = GetKey(attacker, victim);

            if (all.TryGetValue(key, out var hitModelQueue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        long GetKey(IDComponent attacker, IDComponent victim)
        {
            short attackerEntityType = (short)attacker.EntityType;
            short attackerEntityId = (short)attacker.EntityID;

            short victimEntityType = (short)victim.EntityType;
            short victimEntityId = (short)victim.EntityID;

            long key = (long)attackerEntityType << 48;
            key |= (long)attackerEntityId << 32;
            key |= (long)victimEntityType << 16;
            key |= (long)victimEntityId;

            return key;
        }

    }


}
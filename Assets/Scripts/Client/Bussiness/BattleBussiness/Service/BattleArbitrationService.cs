using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public enum AttackTag : byte
    {
        None,
        Enemy,          // - 敌军
        Self,           // -自己
        AllyInculeSelf, // -友军（包括自己）
        AllyExcludeSelf, // -友军（除了自己）
    }

    public struct HitPowerModel
    {
        public int damage;
        public bool canHitRepeatly;
        public AttackTag attackTag;
    }

    public class BattleArbitrationService
    {

        Dictionary<long, Queue<HitPowerModel>> all;

        public BattleArbitrationService()
        {
            all = new Dictionary<long, Queue<HitPowerModel>>();
        }

        public bool CanDamage(IDComponent attacker, IDComponent victim, in HitPowerModel hitPowerModel)
        {
            // 不可多次伤害
            if (HasHitRecord(attacker, victim, hitPowerModel) && !hitPowerModel.canHitRepeatly)
            {
                Debug.LogWarning($"不可多次伤害 ");
                return false;
            }

            // 不是正确的攻击对象
            if (!IsRightTarget(attacker, victim, hitPowerModel))
            {
                Debug.LogWarning($"不是正确的攻击对象 ");
                return false;
            }

            // TODO: 是否可以伤害：  对方是否无敌 是否死亡等等

            return true;

        }

        #region [PRIVATE]

        bool IsRightTarget(IDComponent attacker, IDComponent victim, in HitPowerModel hitPowerModel)
        {

            AttackTag attackTag = hitPowerModel.attackTag;

            if (attackTag == AttackTag.None)
            {
                return false;
            }

            if (attackTag == AttackTag.Enemy)
            {
                if (attacker.LeagueId == 0 && victim.LeagueId == 0)
                {
                    return true;
                }

                if (attacker.LeagueId != victim.LeagueId)
                {
                    return true;
                }

                return false;
            }

            if (attackTag == AttackTag.Self)
            {
                return attacker == victim;
            }

            if (attackTag == AttackTag.AllyInculeSelf)
            {
                return IsAlly(attacker, victim);
            }

            if (attackTag == AttackTag.AllyExcludeSelf)
            {
                if (attacker == victim)
                {
                    return false;
                }

                return IsAlly(attacker, victim);
            }

            Debug.LogWarning("未处理的情况");
            return false;
        }

        bool IsAlly(IDComponent id1, IDComponent id2)
        {
            // 自己也属于友军
            if (id1 != id2 && id1.LeagueId == 0 && id2.LeagueId == 0)
            {
                return false;
            }

            return id1.LeagueId == id2.LeagueId;
        }

        bool HasHitRecord(IDComponent attacker, IDComponent victim, in HitPowerModel hitPowerModel)
        {

            long key = GetKey(attacker, victim);

            if (all.TryGetValue(key, out var modelQueue))
            {
                Debug.Log($"Hit Record Exist! num: {modelQueue.Count}  --------------");
                while (modelQueue.TryDequeue(out var model))
                {
                    Debug.Log($"model damage: {model.damage}");
                }
                return true;
            }
            else
            {
                Debug.Log($"Hit Record Not Exist!");
                return false;
            }

        }

        long GetKey(IDComponent attacker, IDComponent victim)
        {
            short attackerEntityType = (short)attacker.EntityType;
            short attackerEntityId = (short)attacker.EntityId;

            short victimEntityType = (short)victim.EntityType;
            short victimEntityId = (short)victim.EntityId;

            long key = (long)attackerEntityType << 48;
            key |= (long)attackerEntityId << 32;
            key |= (long)victimEntityType << 16;
            key |= (long)victimEntityId;

            return key;
        }

        #endregion

    }


}
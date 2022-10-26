using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness
{

    public enum AttackTag : byte
    {
        None,
        Enemy,          // - 敌军
        Ally, // - 友军
    }

    public class BattleArbitrationService
    {

        Dictionary<long, Queue<HitPowerModel>> all;

        BattleFacades battleFacades;

        public BattleArbitrationService()
        {
            all = new Dictionary<long, Queue<HitPowerModel>>();
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public bool CanHit(IDComponent attacker, IDComponent victim, in HitPowerModel hitPowerModel)
        {

            // 不可多次伤害
            if (HasHitRecord(attacker, victim, hitPowerModel) && !hitPowerModel.canHitRepeatly)
            {
                Debug.LogWarning($"不可多次受击 ");
                return false;
            }

            // 不是正确的攻击对象
            if (!IsRightTarget(attacker, victim, hitPowerModel))
            {
                Debug.LogWarning($"不是正确的受击对象 ");
                return false;
            }

            // TODO: 是否可以伤害：  对方是否无敌 是否死亡等等
            if (!CanDamage(victim))
            {
                Debug.LogWarning($"受击对象 无法被伤害");
                return false;
            }

            return true;

        }

        #region [PRIVATE]

        bool CanDamage(IDComponent victim)
        {
            if (victim.EntityType == EntityType.BattleRole)
            {
                var role = battleFacades.Repo.RoleRepo.Get(victim.EntityID);
                var stateComponent = role.StateComponent;
                return stateComponent.RoleState != RoleState.Reborn && stateComponent.RoleState != RoleState.Dead;
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
            short attackerEntityId = (short)attacker.EntityID;

            short victimEntityType = (short)victim.EntityType;
            short victimEntityId = (short)victim.EntityID;

            long key = (long)attackerEntityType << 48;
            key |= (long)attackerEntityId << 32;
            key |= (long)victimEntityType << 16;
            key |= (long)victimEntityId;

            return key;
        }

        #endregion

    }


}
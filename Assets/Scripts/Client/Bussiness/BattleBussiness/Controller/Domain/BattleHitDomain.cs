using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleHitDomain
    {
        BattleFacades battleFacades;

        public BattleHitDomain()
        {
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public bool TryHitActor(IDComponent attackerIDC, IDComponent victimIDC, in HitPowerModel hitPowerModel, float fixedDeltaTime)
        {
            var arbitService = battleFacades.ArbitrationService;
            if (!arbitService.CanDamage(attackerIDC, victimIDC, hitPowerModel))
            {
                return false;
            }

            ApplyHitRole(attackerIDC, victimIDC, hitPowerModel, fixedDeltaTime);

            return true;

        }

        void ApplyHitRole(IDComponent attackerIDC, IDComponent victimIDC, in HitPowerModel hitPowerModel, float fixedDeltaTime)
        {
            if (victimIDC.EntityType != EntityType.BattleRole)
            {
                return;
            }

            var role = battleFacades.Repo.RoleRepo.GetByEntityId(attackerIDC.EntityId);

            // 作用伤害
            role.HealthComponent.HurtByDamage(hitPowerModel.damage);

            // 作用物理
            role.MoveComponent.AddExtraVelocity(hitPowerModel.hitVelocity);
            role.MoveComponent.Tick_Rigidbody(fixedDeltaTime);

        }

    }

}
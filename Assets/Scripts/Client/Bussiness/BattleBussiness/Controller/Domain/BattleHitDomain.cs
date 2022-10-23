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
            if (!arbitService.CanHit(attackerIDC, victimIDC, hitPowerModel))
            {
                Debug.Log($"不能造成伤害");
                return false;
            }

            ApplyBulletHitRole(attackerIDC, victimIDC, hitPowerModel, fixedDeltaTime);

            Debug.Log($"victimIDC : {victimIDC.EntityType.ToString()}");
            return true;

        }

        void ApplyBulletHitRole(IDComponent attackerIDC, IDComponent victimIDC, in HitPowerModel hitPowerModel, float fixedDeltaTime)
        {
            if (attackerIDC.EntityType != EntityType.Bullet)
            {
                return;
            }

            if (victimIDC.EntityType != EntityType.BattleRole)
            {
                return;
            }

            var bullet = battleFacades.Repo.BulletRepo.Get(attackerIDC.EntityId);
            var role = battleFacades.Repo.RoleRepo.Get(victimIDC.EntityId);

            // 作用伤害
            role.HealthComponent.HurtByDamage(hitPowerModel.damage);

            // 作用物理
            var addV = bullet.MoveComponent.Velocity * hitPowerModel.hitVelocityCoefficient;
            role.MoveComponent.AddExtraVelocity(addV);
            role.MoveComponent.Tick_Rigidbody(fixedDeltaTime);

            // 状态
            role.StateComponent.EnterBeHit(hitPowerModel.freezeMaintainFrame);

        }

    }

}
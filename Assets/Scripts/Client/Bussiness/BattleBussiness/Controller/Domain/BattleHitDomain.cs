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
            if (!arbitService.IsHitSuccess(attackerIDC, victimIDC, hitPowerModel))
            {
                Debug.Log($"打击失败!");
                return false;
            }

            // - Record
            arbitService.AddHitRecord(attackerIDC, victimIDC);

            // - Hit Apply
            ApplyBulletHitRole(attackerIDC, victimIDC, hitPowerModel, fixedDeltaTime);

            return true;
        }

        void ApplyBulletHitRole(IDComponent attackerIDC, IDComponent victimIDC, in HitPowerModel hitPowerModel, float fixedDeltaTime)
        {
            if (attackerIDC.EntityType != EntityType.Bullet
            || victimIDC.EntityType != EntityType.BattleRole)
            {
                return;
            }

            var bullet = battleFacades.Repo.BulletRepo.Get(attackerIDC.EntityID);
            var role = battleFacades.Repo.RoleRepo.Get(victimIDC.EntityID);

            if (bullet.BulletType == BulletType.DefaultBullet)
            {
                // 作用伤害
                role.TryReceiveDamage(hitPowerModel.damage);
                // 作用物理
                var addV = bullet.MoveComponent.Velocity * hitPowerModel.hitVelocityCoefficient;
                role.MoveComponent.AddExtraVelocity(addV);
                role.MoveComponent.Tick_Rigidbody(fixedDeltaTime);
            }

            // 状态
            role.StateComponent.EnterBeHit(hitPowerModel.freezeMaintainFrame);
        }

    }

}
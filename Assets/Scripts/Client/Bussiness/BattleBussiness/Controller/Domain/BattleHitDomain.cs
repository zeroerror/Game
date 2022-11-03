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

        public bool TryHitActor(IDComponent attackerIDC, IDComponent victimIDC, in HitPowerModel hitPowerModel)
        {
            var arbitService = battleFacades.ArbitrationService;
            if (!arbitService.IsHitSuccess(attackerIDC, victimIDC, hitPowerModel))
            {
                Debug.Log($"打击失败!");
                return false;
            }

            // - Hit Apply
            ApplyBulletHitRole(attackerIDC, victimIDC, hitPowerModel);

            return true;
        }

        void ApplyBulletHitRole(IDComponent attackerIDC, IDComponent victimIDC, in HitPowerModel hitPowerModel)
        {
            if (attackerIDC.EntityType != EntityType.Bullet
            || victimIDC.EntityType != EntityType.BattleRole)
            {
                return;
            }

            var role = battleFacades.Repo.RoleLogicRepo.Get(victimIDC.EntityID);
            CauseAndRecordDamage(attackerIDC, victimIDC, hitPowerModel.damage, role);

            var bullet = battleFacades.Repo.BulletRepo.Get(attackerIDC.EntityID);
            CausePhysics(role, bullet, hitPowerModel.knockBackSpeed, hitPowerModel.blowUpSpeed);

            // - State
            role.StateComponent.EnterBeHit(hitPowerModel.freezeMaintainFrame);
        }

        void CausePhysics(BattleRoleLogicEntity role, BulletEntity bullet, float knockBackSpeed, float blowUpSpeed)
        {
            if (bullet is GrenadeEntity grenadeEntity && !grenadeEntity.isTrigger)
            {
                return;
            }

            // - Physics
            var dir = role.LocomotionComponent.Position - bullet.LocomotionComponent.Position;
            var addV = dir.normalized * knockBackSpeed;
            addV.y += blowUpSpeed;
            role.LocomotionComponent.AddExtraVelocity(addV);
        }

        void CauseAndRecordDamage(IDComponent attackerIDC, IDComponent victimIDC, float damage, BattleRoleLogicEntity role)
        {
            var arbitService = battleFacades.ArbitrationService;
            var domain = battleFacades.Domain.RoleDomain;
            float receivedDamage = domain.TryReceiveDamage(role, damage);
            arbitService.AddHitRecord(attackerIDC, victimIDC, receivedDamage);
        }
        
    }

}
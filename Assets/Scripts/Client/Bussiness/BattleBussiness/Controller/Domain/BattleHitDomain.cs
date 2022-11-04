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
            TryApplyBulletHitRole(attackerIDC, victimIDC, hitPowerModel);

            return true;
        }

        void TryApplyBulletHitRole(IDComponent attackerIDC, IDComponent victimIDC, in HitPowerModel hitPowerModel)
        {
            if (attackerIDC.EntityType != EntityType.Bullet
            || victimIDC.EntityType != EntityType.BattleRole)
            {
                return;
            }

            // - Damage Coefficient
            var bullet = battleFacades.Repo.BulletRepo.Get(attackerIDC.EntityID);
            var role = battleFacades.Repo.RoleLogicRepo.Get(victimIDC.EntityID);

            CausePhysics(role, bullet, hitPowerModel.knockBackSpeed, hitPowerModel.blowUpSpeed);

            var repo = battleFacades.Repo;
            var roleRepo = repo.RoleLogicRepo;
            var master = roleRepo.Get(bullet.MasterEntityID);
            var weaponComponent = master.WeaponComponent;
            var bulletDamage = bullet.GetDamageByCoefficient(weaponComponent.DamageCoefficient);
            CauseAndRecordDamage(attackerIDC, victimIDC, bulletDamage, role);

            // - State
            role.StateComponent.EnterBeHit(hitPowerModel.freezeMaintainFrame);

            return;
        }

        void CausePhysics(BattleRoleLogicEntity role, BulletEntity bullet, float knockBackSpeed, float blowUpSpeed)
        {
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

            // -  Logic Trigger
            var logicTriggerAPI = battleFacades.LogicTriggerAPI;
            DamageRecordArgs args;
            args.atkEntityType = attackerIDC.EntityType;
            args.atkEntityID = attackerIDC.EntityID;
            args.vicEntityType = victimIDC.EntityType;
            args.vicEntityID = victimIDC.EntityID;
            args.damage = receivedDamage;
            logicTriggerAPI.Invoke_DamageRecordAction(args);
        }

    }

}
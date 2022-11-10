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
            ApplyBulletHit(attackerIDC, victimIDC, hitPowerModel);

            return true;
        }

        void ApplyBulletHit(IDComponent atkIDC, IDComponent victimIDC, in HitPowerModel hitPowerModel)
        {
            var atkEntityType = atkIDC.EntityType;
            var atkEntityID = atkIDC.EntityID;
            if (atkEntityType != EntityType.Bullet)
            {
                return;
            }

            EntityType victimEntityType = victimIDC.EntityType;
            int victimEntityID = victimIDC.EntityID;
            LocomotionComponent victimLC = null;

            if (victimEntityType == EntityType.BattleRole)
            {
                var role = battleFacades.Repo.RoleLogicRepo.Get(victimEntityID);
                victimLC = role.LocomotionComponent;
            }
            else if (victimEntityType == EntityType.Aridrop)
            {
                var airdrop = battleFacades.Repo.AirdropLogicRepo.Get(victimEntityID);
                victimLC = airdrop.LocomotionComponent;
            }
            else
            {
                Debug.LogError("Not Handler");
            }

            var bullet = battleFacades.Repo.BulletLogicRepo.Get(atkEntityID);

            // - Physics
            var bulletLC = bullet.LocomotionComponent;
            CausePhysics(bulletLC, victimLC, hitPowerModel.knockBackSpeed, hitPowerModel.blowUpSpeed);

            // - Damage
            var repo = battleFacades.Repo;
            var weaponRepo = repo.WeaponRepo;
            var weapon = weaponRepo.Get(bullet.WeaponID);
            var bulletDamage = bullet.GetDamageByCoefficient(weapon.DamageCoefficient);
            CauseDamage(atkIDC, victimIDC, bulletDamage, out float receivedDamage, out bool hasCausedDeath);

            // - Recording
            var arbitService = battleFacades.ArbitrationService;
            arbitService.AddHitRecord(atkIDC, victimIDC, receivedDamage, hasCausedDeath);

            // -  Logic Trigger
            var logicEventCenter = battleFacades.LogicEventCenter;
            DamageRecordArgs args;
            args.atkEntityType = atkEntityType;
            args.atkEntityID = atkEntityID;
            args.vicEntityType = victimEntityType;
            args.vicEntityID = victimEntityID;
            args.damage = receivedDamage;
            logicEventCenter.Invoke_BattleDamageRecordAction(args);

            return;
        }

        void CausePhysics(LocomotionComponent atkLC, LocomotionComponent victimLC, float knockBackSpeed, float blowUpSpeed)
        {
            // - Physics
            var dir = victimLC.Position - atkLC.Position;
            var addV = dir.normalized * knockBackSpeed;
            addV.y += blowUpSpeed;
            victimLC.AddExtraVelocity(addV);
        }

        void CauseDamage(IDComponent attackerIDC, IDComponent victimIDC, float damage, out float receivedDamage, out bool hasCausedDeath)
        {
            receivedDamage = 0;
            hasCausedDeath = false;

            var repo = battleFacades.Repo;

            if (victimIDC.EntityType == EntityType.BattleRole)
            {
                var roleRepo = repo.RoleLogicRepo;
                var role = roleRepo.Get(victimIDC.EntityID);
                var roleDomain = battleFacades.Domain.RoleLogicDomain;
                receivedDamage = roleDomain.TryReceiveDamage(role, damage);
                hasCausedDeath = role.HealthComponent.CheckIsDead();
                return;
            }

            if (victimIDC.EntityType == EntityType.Aridrop)
            {
                var airdropLogicRepo = repo.AirdropLogicRepo;
                var airdropLogic = airdropLogicRepo.Get(victimIDC.EntityID);
                var airdropLogicDomain = battleFacades.Domain.AirdropLogicDomain;
                receivedDamage = airdropLogicDomain.TryReceiveDamage(airdropLogic, damage);
                hasCausedDeath = airdropLogic.HealthComponent.CheckIsDead();
                return;
            }

            Debug.LogError("未处理");
        }

    }

}
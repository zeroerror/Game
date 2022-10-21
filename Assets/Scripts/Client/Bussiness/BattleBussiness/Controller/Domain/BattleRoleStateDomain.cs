using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Network;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleRoleStateDomain
    {

        BattleFacades battleFacades;

        public BattleRoleStateDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public void ApplyAllRoleState()
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                ApplyRoleState(role);
                role.InputComponent.Reset();
            });
        }

        public void ApplyRoleState(BattleRoleLogicEntity roleLogicEntity)
        {
            ApplyAnyState(roleLogicEntity);
            ApplyNormal(roleLogicEntity);
            ApplyRolling(roleLogicEntity);
            ApplyClimbing(roleLogicEntity);
            ApplyFiring(roleLogicEntity);
            ApplyReloading(roleLogicEntity);
            ApplyHealing(roleLogicEntity);
            ApplySwitching(roleLogicEntity);
            ApplyBeHit(roleLogicEntity);
            ApplyDead(roleLogicEntity);
            ApplyReborn(roleLogicEntity);
        }

        void ApplyAnyState(BattleRoleLogicEntity role)
        {
            var inputComponent = role.InputComponent;
            var roleDomain = battleFacades.Domain.RoleDomain;

            // Locomotion
            bool hasMoveDir = inputComponent.MoveDir != Vector3.zero;
            if (hasMoveDir)
            {
                roleDomain.RoleMoveActivate(role, inputComponent.MoveDir);
            }

        }

        void ApplyNormal(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Normal)
            {
                return;
            }

            var inputComponent = role.InputComponent;

            var roleDomain = battleFacades.Domain.RoleDomain;
            bool hasRollDir = inputComponent.RollDir != Vector3.zero;
            if (hasRollDir && roleDomain.TryRoleRoll(role, inputComponent.RollDir))
            {
                stateComponent.EnterRolling(40);
                return;
            }

            if (inputComponent.pressReload)
            {
                var weaponComponent = role.WeaponComponent;
                role.WeaponComponent.BeginReloading();
                stateComponent.EnterReloading(weaponComponent.CurrentWeapon.ReloadFrame);
                return;
            }

            var moveComponent = role.MoveComponent;
            moveComponent.SetEulerAngle(inputComponent.FaceDir);

        }

        void ApplyRolling(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Rolling)
            {
                return;
            }

            var stateMod = stateComponent.RollingMod;
            if (stateMod.maintainFrame <= 0)
            {
                stateComponent.EnterNormal();
                return;
            }

            stateMod.maintainFrame--;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;
            }

            var moveComponent = role.MoveComponent;
            moveComponent.SetMoveVelocity(Vector3.zero);

            var inputComponent = role.InputComponent;
            moveComponent.SetEulerAngle(inputComponent.RollDir);
        }

        void ApplyReloading(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Reloading)
            {
                return;
            }

            var stateMod = stateComponent.ReloadingMod;
            if (stateMod.maintainFrame <= 0)
            {
                stateComponent.EnterNormal();
                return;
            }

            stateMod.maintainFrame--;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;

            }

            // Locomotion
            var moveComponent = role.MoveComponent;
            moveComponent.SetMoveVelocity(moveComponent.MoveVelocity / 4f);
            var inputComponent = role.InputComponent;
            moveComponent.SetEulerAngle(inputComponent.FaceDir);
        }

        void ApplyFiring(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Shooting)
            {
                return;
            }

            var stateMod = stateComponent.ShootingMod;
            if (stateMod.maintainFrame <= 0)
            {
                stateComponent.EnterNormal();
                return;
            }

            stateMod.maintainFrame--;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;
            }

            // Locomotion
            var roleDomain = battleFacades.Domain.RoleDomain;
            var moveComponent = role.MoveComponent;
            moveComponent.SetMoveVelocity(moveComponent.MoveVelocity / 4f);

            var inputComponent = role.InputComponent;
            moveComponent.SetEulerAngle(inputComponent.FireDir);
        }

        void ApplyBeHit(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.BeHit)
            {
                return;
            }

            var stateMod = stateComponent.BeHitMod;
            if (stateMod.maintainFrame <= 0)
            {
                stateComponent.EnterNormal();
                return;
            }

            stateMod.maintainFrame--;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;

            }

            var roleDomain = battleFacades.Domain.RoleDomain;
            var moveComponent = role.MoveComponent;
            roleDomain.RoleMoveActivate(role, moveComponent.MoveVelocity / 3f);

        }

        void ApplyClimbing(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Climbing)
            {
                return;
            }

            var stateMod = stateComponent.ClimbingMod;
            if (stateMod.maintainFrame <= 0)
            {
                stateComponent.EnterNormal();
                return;
            }

            stateMod.maintainFrame--;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;

            }
        }

        void ApplyDead(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Dead)
            {
                return;
            }

            var roleDomain = battleFacades.Domain.RoleDomain;
            var stateMod = stateComponent.DeadMod;
            if (stateMod.maintainFrame <= 0)
            {
                stateComponent.EnterReborn(30);
                roleDomain.RoleReborn(role);
                return;
            }

            stateMod.maintainFrame--;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;

            }

            var moveComponent = role.MoveComponent;
            moveComponent.SetMoveVelocity(Vector3.zero);
        }

        void ApplyReborn(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Reborn)
            {
                return;
            }

            var stateMod = stateComponent.RebornMod;
            if (stateMod.maintainFrame <= 0)
            {
                stateComponent.EnterNormal();
                return;
            }

            stateMod.maintainFrame--;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;
            }
        }

        void ApplyHealing(BattleRoleLogicEntity role)
        {

        }

        void ApplySwitching(BattleRoleLogicEntity role)
        {

        }

    }

}
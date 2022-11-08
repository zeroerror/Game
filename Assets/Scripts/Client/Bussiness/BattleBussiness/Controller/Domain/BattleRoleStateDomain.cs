using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;

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
            var roleRepo = battleFacades.Repo.RoleLogicRepo;
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
            var roleDomain = battleFacades.Domain.RoleLogicDomain;

            // Locomotion
            role.LocomotionComponent.ApplyMoveVelocity(inputComponent.MoveDir);

        }

        void ApplyNormal(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Normal)
            {
                return;
            }

            var inputComponent = role.InputComponent;

            if (role.TryRoll(inputComponent.RollDir))
            {
                return;
            }

            if (inputComponent.pressReload)
            {
                var weaponComponent = role.WeaponComponent;
                if (role.CanWeaponReload())
                {
                    weaponComponent.BeginReloading();
                    stateComponent.EnterReloading(weaponComponent.CurWeapon.ReloadFrame);
                }

                return;
            }

            var moveComponent = role.LocomotionComponent;
            moveComponent.SetRotation(inputComponent.FaceDir);

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

            var moveComponent = role.LocomotionComponent;
            moveComponent.SetMoveVelocity(Vector3.zero);

            var inputComponent = role.InputComponent;
            moveComponent.SetRotation(inputComponent.RollDir);
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
            var moveComponent = role.LocomotionComponent;
            moveComponent.SetMoveVelocity(moveComponent.MoveVelocity * 0.7f);
            var inputComponent = role.InputComponent;
            moveComponent.SetRotation(inputComponent.FaceDir);
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
            var roleDomain = battleFacades.Domain.RoleLogicDomain;
            var moveComponent = role.LocomotionComponent;
            moveComponent.SetMoveVelocity(moveComponent.MoveVelocity * 0.7f);

            var inputComponent = role.InputComponent;
            moveComponent.SetRotation(inputComponent.ShootDir);
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

            var moveComponent = role.LocomotionComponent;
            moveComponent.ApplyMoveVelocity(moveComponent.MoveVelocity * 0.7f);
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
            if (stateComponent.RoleState != RoleState.Dying)
            {
                return;
            }

            var roleDomain = battleFacades.Domain.RoleLogicDomain;
            var stateMod = stateComponent.DeadMod;
            if (stateMod.maintainFrame <= 0)
            {
                stateComponent.EnterReborn(30);
                roleDomain.Reborn(role);
                return;
            }

            stateMod.maintainFrame--;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;

            }

            var moveComponent = role.LocomotionComponent;
            moveComponent.SetMoveVelocity(Vector3.zero);
        }

        void ApplyReborn(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Reborning)
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
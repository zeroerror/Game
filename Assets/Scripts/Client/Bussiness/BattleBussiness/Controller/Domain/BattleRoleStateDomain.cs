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
            ApplyShooting(roleLogicEntity);
            ApplyReloading(roleLogicEntity);
            ApplyHealing(roleLogicEntity);
            ApplySwitching(roleLogicEntity);
            ApplyBeHit(roleLogicEntity);
            ApplyDead(roleLogicEntity);
        }

        void ApplyAnyState(BattleRoleLogicEntity role)
        {
            var inputComponent = role.InputComponent;
            var moveComponent = role.MoveComponent;

            moveComponent.SetEulerAngle(inputComponent.RotEuler);
        }

        void ApplyNormal(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Normal)
            {
                return;
            }

            var moveComponent = role.MoveComponent;
            var weaponComponent = role.WeaponComponent;
            var roleInputComponent = role.InputComponent;

            bool hasMoveDir = roleInputComponent.MoveDir != Vector3.zero;
            bool hasRollDir = roleInputComponent.RollDir != Vector3.zero;

            var roleDomain = battleFacades.Domain.RoleDomain;
            if (hasRollDir && roleDomain.TryRoleRoll(role, roleInputComponent.RollDir))
            {
                stateComponent.EnterRolling(20);
                return;
            }

            if (roleInputComponent.pressReload)
            {
                stateComponent.EnterReloading(30);
                return;
            }

            if (hasMoveDir)
            {
                roleDomain.RoleMove(role, roleInputComponent.MoveDir);
            }

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

        void ApplyHealing(BattleRoleLogicEntity role)
        {

        }

        void ApplySwitching(BattleRoleLogicEntity role)
        {

        }

        void ApplyDead(BattleRoleLogicEntity role)
        {

        }

        void ApplyShooting(BattleRoleLogicEntity role)
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
        }

    }

}
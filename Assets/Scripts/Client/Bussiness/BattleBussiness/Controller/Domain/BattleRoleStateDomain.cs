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

        public void ApplyRoleState(BattleRoleLogicEntity roleLogicEntity)
        {
            ApplyAnyState(roleLogicEntity);
            ApplyNormal(roleLogicEntity);
            ApplyRolling(roleLogicEntity);
            ApplyClimbing(roleLogicEntity);
            ApplyAttacking(roleLogicEntity);
            ApplyReloading(roleLogicEntity);
            ApplyHealing(roleLogicEntity);
            ApplySwitching(roleLogicEntity);
            ApplyBeHit(roleLogicEntity);
            ApplyDead(roleLogicEntity);

        }

        void ApplyAnyState(BattleRoleLogicEntity roleLogicEntity)
        {
            // 设定: 任何状态都存在物理

        }

        void ApplyNormal(BattleRoleLogicEntity roleLogicEntity)
        {
            var stateComponent = roleLogicEntity.StateComponent;
            if (stateComponent.RoleState != RoleState.Normal)
            {
                return;
            }

            var moveComponent = roleLogicEntity.MoveComponent;
            var weaponComponent = roleLogicEntity.WeaponComponent;
            var inputComponent = roleLogicEntity.InputComponent;
            var animatorComponent = roleLogicEntity.roleRenderer.AnimatorComponent;

            // Logic
            bool hasMoveDir = inputComponent.moveDir != Vector3.zero;
            if (hasMoveDir)
            {
                roleLogicEntity.MoveComponent.ActivateMoveVelocity(inputComponent.moveDir);
            }

            // Renderer
            if (hasMoveDir)
            {
                if (moveComponent.IsGrounded && weaponComponent.CurrentWeapon == null)
                {
                    animatorComponent.PlayRunning();
                }
                if (moveComponent.IsGrounded && weaponComponent.CurrentWeapon != null)
                {
                    animatorComponent.PlayRunWithGun();
                }
            }
            else
            {
                animatorComponent.PlayIdle();
            }

        }

        void ApplyRolling(BattleRoleLogicEntity roleLogicEntity)
        {
            var animatorComponent = roleLogicEntity.roleRenderer.AnimatorComponent;
            animatorComponent.PlayRollForward();
        }

        void ApplyClimbing(BattleRoleLogicEntity roleLogicEntity)
        {

        }

        void ApplyAttacking(BattleRoleLogicEntity roleLogicEntity)
        {

        }

        void ApplyReloading(BattleRoleLogicEntity roleLogicEntity)
        {

        }

        void ApplyHealing(BattleRoleLogicEntity roleLogicEntity)
        {

        }

        void ApplySwitching(BattleRoleLogicEntity roleLogicEntity)
        {

        }

        void ApplyBeHit(BattleRoleLogicEntity roleLogicEntity)
        {

        }

        void ApplyDead(BattleRoleLogicEntity roleLogicEntity)
        {

        }

    }

}
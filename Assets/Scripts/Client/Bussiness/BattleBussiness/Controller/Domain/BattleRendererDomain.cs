

using Game.Client.Bussiness.BattleBussiness.Facades;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleRendererDomain
    {

        BattleFacades battleFacades;

        public BattleRendererDomain()
        {

        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void ApplyRoleStateRenderer(float deltaTime)
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                ApplyRoleState_Any(role, deltaTime);
                ApplyRoleState_Normal(role);
                ApplyRoleState_Rolling(role);
                ApplyRoleState_Reloading(role);
                ApplyRoleState_Shooting(role);

            });
        }

        void ApplyRoleState_Any(BattleRoleLogicEntity role, float deltaTime)
        {
            var moveComponent = role.MoveComponent;
            var roleRenderer = role.roleRenderer;
            roleRenderer.transform.position = Vector3.Lerp(roleRenderer.transform.position, moveComponent.Position, deltaTime * roleRenderer.posAdjust);
            roleRenderer.transform.rotation = Quaternion.Lerp(roleRenderer.transform.rotation, moveComponent.Rotation, deltaTime * roleRenderer.rotAdjust);

            bool isPosChange = roleRenderer.transform.position != moveComponent.Position;
            if (isPosChange)
            {
                role.roleRenderer.noMoveTime = 0;
            }
        }

        void ApplyRoleState_Normal(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Normal)
            {
                return;
            }

            var moveComponent = role.MoveComponent;
            var weaponComponent = role.WeaponComponent;
            var roleRenderer = role.roleRenderer;
            var animatorComponent = roleRenderer.AnimatorComponent;
            var roleState = role.StateComponent.RoleState;

            // 1. Idle 2. Running 3. IdleWithGun 4. RunningWithGun
            bool isHoldingGun = weaponComponent.CurrentWeapon != null;
            bool isPosChange = Vector3.Distance(roleRenderer.transform.position, moveComponent.Position) > 0.03f;

            if (!isPosChange && !isHoldingGun)
            {
                if (!animatorComponent.IsInState("Idle"))
                {
                    animatorComponent.PlayIdle();
                }
                return;
            }

            if (isPosChange && !isHoldingGun)
            {
                if (!animatorComponent.IsInState("Running"))
                {
                    animatorComponent.PlayRunning();
                }
                return;
            }

            if (!isPosChange && isHoldingGun)
            {
                if (!animatorComponent.IsInState("Idle_With_Gun"))
                {
                    animatorComponent.PlayIdleWithGun();
                }
                return;
            }

            if (isPosChange && isHoldingGun)
            {
                if (!animatorComponent.IsInState("Running_With_Gun"))
                {
                    animatorComponent.PlayRunnigWithGun();
                }
                return;
            }

        }

        void ApplyRoleState_Rolling(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Rolling)
            {
                return;
            }

            var roleRenderer = role.roleRenderer;
            var animatorComponent = roleRenderer.AnimatorComponent;
            if (!animatorComponent.IsInState("Rolling"))
            {
                animatorComponent.PlayRolling();
            }
        }

        void ApplyRoleState_Reloading(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Reloading)
            {
                return;
            }

            var roleRenderer = role.roleRenderer;
            var animatorComponent = roleRenderer.AnimatorComponent;
            if (!animatorComponent.IsInState("Reloading"))
            {
                animatorComponent.PlayReloading();
            }
        }

        void ApplyRoleState_Shooting(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Shooting)
            {
                return;
            }

            var roleRenderer = role.roleRenderer;
            var animatorComponent = roleRenderer.AnimatorComponent;
            animatorComponent.PlayShooting();
        }

    }

}

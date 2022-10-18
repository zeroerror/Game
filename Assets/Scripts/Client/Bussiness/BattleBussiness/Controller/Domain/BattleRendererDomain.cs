

using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Generic;
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
                ApplyRoleState_Dead(role);
                ApplyRoleState_Reborn(role);
            });
        }

        void ApplyRoleState_Any(BattleRoleLogicEntity role, float deltaTime)
        {
            var moveComponent = role.MoveComponent;
            var roleRenderer = role.roleRenderer;
            roleRenderer.transform.position = Vector3.Lerp(roleRenderer.transform.position, moveComponent.Position, deltaTime * roleRenderer.posAdjust);
            roleRenderer.transform.rotation = Quaternion.Lerp(roleRenderer.transform.rotation, moveComponent.Rotation, deltaTime * roleRenderer.rotAdjust);

            bool isPosChange = !roleRenderer.transform.position.MostEqualsY(moveComponent.Position);
            if (isPosChange)
            {
                role.roleRenderer.noMoveTime = 0;
            }
            else
            {
                role.roleRenderer.noMoveTime++;
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
            bool isPosChange = Vector3.Distance(roleRenderer.transform.position, moveComponent.Position) > 0.05f;

            if (!isPosChange && !isHoldingGun)
            {
                if (!animatorComponent.IsInState("Idle") && roleRenderer.noMoveTime > 0.1f)
                {
                    Debug.Log("PlayIdle Idle ");
                    animatorComponent.PlayIdle();
                }
                return;
            }

            if (isPosChange && !isHoldingGun)
            {
                if (!animatorComponent.IsInState("Run"))
                {
                    Debug.Log("PlayRun Run ");
                    animatorComponent.PlayRun();
                }
                return;
            }

            if (!isPosChange && isHoldingGun)
            {
                if (!animatorComponent.IsInState("Idle_Rifle") && roleRenderer.noMoveTime > 0.1f)
                {
                    animatorComponent.PlayIdle_Rifle();
                }
                return;
            }

            if (isPosChange && isHoldingGun)
            {
                if (!animatorComponent.IsInState("Run_Rifle"))
                {
                    animatorComponent.PlayRun_Rifle();
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
            if (!animatorComponent.IsInState("Roll"))
            {
                animatorComponent.PlayRoll();
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
            if (!animatorComponent.IsInState("Reload"))
            {
                animatorComponent.PlayReload_Run();
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
            animatorComponent.PlayShoot_Rifle();
        }

        void ApplyRoleState_Dead(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Dead)
            {
                return;
            }

            var roleRenderer = role.roleRenderer;
            var animatorComponent = roleRenderer.AnimatorComponent;
            if (!animatorComponent.IsInState("Dead"))
            {
                animatorComponent.PlayDead();
            }
        }

        void ApplyRoleState_Reborn(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Reborn)
            {
                return;
            }

            var roleRenderer = role.roleRenderer;
            var animatorComponent = roleRenderer.AnimatorComponent;
            if (!animatorComponent.IsInState("Roll"))
            {
                animatorComponent.PlayRoll();    // TODO Reborn动画
            }
        }

    }

}

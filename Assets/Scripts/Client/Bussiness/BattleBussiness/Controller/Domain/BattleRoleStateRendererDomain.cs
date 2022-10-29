

using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleRoleStateRendererDomain
    {

        BattleFacades battleFacades;

        public BattleRoleStateRendererDomain()
        {

        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void ApplyRoleState(float deltaTime)
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                ApplyAny(role, deltaTime);
                ApplyNormal(role);
                ApplyRoll(role);
                ApplyReload(role);
                ApplyShoot(role);
                ApplyDead(role);
                ApplyReborn(role);
            });
        }

        void ApplyAny(BattleRoleLogicEntity role, float deltaTime)
        {
            var moveComponent = role.MoveComponent;
            var roleRenderer = role.roleRenderer;

            bool isPosChange = !roleRenderer.transform.position.MostEquals(moveComponent.Position);

            if (isPosChange)
            {
                roleRenderer.staticTime = 0;
            }
            else
            {
                roleRenderer.staticTime += UnityEngine.Time.deltaTime;
            }

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
            var roleRenderer = role.roleRenderer;
            var animatorComponent = roleRenderer.AnimatorComponent;
            var roleState = role.StateComponent.RoleState;

            // 1. Idle 2. Running 3. IdleWithGun 4. RunningWithGun
            bool isHoldingGun = weaponComponent.CurrentWeapon != null;

            if (!IsMoving(role) && !isHoldingGun)
            {
                if (!animatorComponent.IsInState("Idle") && !IsMoving(role))
                {
                    Debug.Log("PlayIdle Idle ");
                    animatorComponent.PlayIdle();
                }
                return;
            }

            if (IsMoving(role) && !isHoldingGun)
            {
                if (!animatorComponent.IsInState("Run"))
                {
                    Debug.Log("PlayRun Run ");
                    animatorComponent.PlayRun();
                }
                return;
            }

            if (!IsMoving(role) && isHoldingGun)
            {
                if (!animatorComponent.IsInState("Idle_Rifle") && !IsMoving(role))
                {
                    animatorComponent.PlayIdle_Rifle();
                }
                return;
            }

            if (IsMoving(role) && isHoldingGun)
            {
                if (!animatorComponent.IsInState("Run_Rifle"))
                {
                    animatorComponent.PlayRun_Rifle();
                }
                return;
            }

        }

        void ApplyRoll(BattleRoleLogicEntity role)
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

        void ApplyReload(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Reloading)
            {
                return;
            }

            var roleRenderer = role.roleRenderer;
            var animatorComponent = roleRenderer.AnimatorComponent;

            if (IsMoving(role) && !animatorComponent.IsInState("Reload_Run"))
            {
                animatorComponent.PlayReload_Run();
                return;
            }

            if (!IsMoving(role) && !animatorComponent.IsInState("Reload"))
            {
                animatorComponent.PlayReload();
                return;
            }
        }

        void ApplyShoot(BattleRoleLogicEntity role)
        {
            var stateComponent = role.StateComponent;
            if (stateComponent.RoleState != RoleState.Shoot)
            {
                return;
            }

            var roleRenderer = role.roleRenderer;
            var animatorComponent = roleRenderer.AnimatorComponent;

            if (!IsMoving(role) && !animatorComponent.IsInState("Shoot_Rifle"))
            {
                animatorComponent.PlayShoot_Rifle();
                return;
            }

            if (IsMoving(role) && !animatorComponent.IsInState("Shoot_Rifle_Run"))
            {
                animatorComponent.PlayShoot_Rifle_Run();
                return;
            }

        }

        void ApplyDead(BattleRoleLogicEntity role)
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

        void ApplyReborn(BattleRoleLogicEntity role)
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

        bool IsMoving(BattleRoleLogicEntity role)
        {
            return role.roleRenderer.staticTime < 0.07f;
        }

    }

}

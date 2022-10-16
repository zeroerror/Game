using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.BattleBussiness.Shot;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public class RoleStateComponent
    {

        [SerializeField]
        RoleState roleState;
        public RoleState RoleState => roleState;
        public void SetRoleState(RoleState roleState) => this.roleState = roleState;

        // Model
        [SerializeField]
        RoleStateRollingMod rollingMod;
        public RoleStateRollingMod RollingMod => rollingMod;

        [SerializeField]
        RoleStateShootingMod shootingMod;
        public RoleStateShootingMod ShootingMod => shootingMod;

        [SerializeField]
        RoleStateReloadingMod reloadingMod;
        public RoleStateReloadingMod ReloadingMod => reloadingMod;

        [SerializeField]
        RoleStateBeHitMod beHitMod;
        public RoleStateBeHitMod BeHitMod => beHitMod;

        [SerializeField]
        RoleStateClimbingMod climbingMod;
        public RoleStateClimbingMod ClimbingMod => climbingMod;

        [SerializeField]
        RoleStateSwitchingMod roleStateSwitchingMod;
        public RoleStateSwitchingMod RoleStateSwitchingMod => roleStateSwitchingMod;

        public void Reset()
        {
            roleState = RoleState.Normal;

            rollingMod = new RoleStateRollingMod();
            climbingMod = new RoleStateClimbingMod();
            reloadingMod = new RoleStateReloadingMod();
            roleStateSwitchingMod = new RoleStateSwitchingMod();
            beHitMod = new RoleStateBeHitMod();
            shootingMod = new RoleStateShootingMod();
        }


        #region [STATE ENTER]

        public void EnterNormal()
        {
            roleState = RoleState.Normal;
        }

        public void EnterRolling(int maintainFrame)
        {
            roleState = RoleState.Rolling;

            rollingMod.isFirstEnter = true;
            rollingMod.maintainFrame = maintainFrame;
        }

        public void EnterShooting(int maintainFrame)
        {
            roleState = RoleState.Shooting;

            shootingMod.isFirstEnter = true;
            shootingMod.maintainFrame = maintainFrame;
        }

        public void EnterReloading(int maintainFrame)
        {
            roleState = RoleState.Reloading;

            reloadingMod.isFirstEnter = true;
            reloadingMod.maintainFrame = maintainFrame;
        }

        public void EnterBeHit(int maintainFrame)
        {
            roleState = RoleState.BeHit;

            beHitMod.isFirstEnter = true;
            beHitMod.maintainFrame = maintainFrame;
        }

        #endregion

    }

}
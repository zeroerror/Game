
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{
    public struct HitPowerModel
    {
        public bool canHitRepeatly;
        public AttackTag attackTag;
        public int damage;
        public Vector3 hitVelocity;
        public int freezeMaintainFrame;
    }

    public struct HitModel
    {
        public IDComponent attackerIDC;
        public IDComponent victimIDC;
    }

    public struct HitFieldModel
    {
        public IDComponent hitter;
        public CollisionExtra fieldCE;
    }

    [Serializable]
    public class RoleStateRollingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    [Serializable]
    public class RoleStateClimbingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    [Serializable]
    public class RoleStateReloadingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    [Serializable]
    public class RoleStateSwitchingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    [Serializable]
    public class RoleStateShootingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    [Serializable]
    public class RoleStateBeHitMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    [Serializable]
    public class RoleStateDeadMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    [Serializable]
    public class RoleStateRebornMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

}
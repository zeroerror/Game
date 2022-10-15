
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

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

    public class RoleStateRollingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    public class RoleStateClimbingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    public class RoleStateReloadingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    public class RoleStateSwitchingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    public class RoleStateShootingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    public class RoleStateBeHitMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

    public class RoleStateAttackingMod
    {
        public bool isFirstEnter;
        public int maintainFrame;
    }

}
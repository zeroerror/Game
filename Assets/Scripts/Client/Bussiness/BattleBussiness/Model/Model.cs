
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

}
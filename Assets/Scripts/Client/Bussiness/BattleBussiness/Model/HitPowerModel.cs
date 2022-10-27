using UnityEngine;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public struct HitPowerModel
    {
        public bool canHitRepeatly;

        public AttackTag attackTag;

        public int damage;

        [Header("撞击速度系数")]
        public float hitVelocityCoefficient;

        public int freezeMaintainFrame;
    }

}
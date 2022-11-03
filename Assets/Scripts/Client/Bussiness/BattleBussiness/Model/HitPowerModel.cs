using UnityEngine;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public struct HitPowerModel
    {
        public bool canHitRepeatly;

        public AttackTag attackTag;

        public float damage;

        [Header("击退速度")]
        public float knockBackSpeed;

        [Header("击飞速度")]
        public float blowUpSpeed;

        public int freezeMaintainFrame;
    }

}
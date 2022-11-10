using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class GrenadeLogicEntity : BulletLogicEntity
    {

        [SerializeField] float explosionRadius;
        public float ExplosionRadius => explosionRadius;

        bool isExploded;
        public bool IsExploded => isExploded;
        public void SetIsExploded(bool v) => isExploded = v;

        protected override void Init()
        {
            base.Init();
        }

    }

}
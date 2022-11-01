using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class GrenadeEntity : BulletEntity
    {

        [SerializeField] float explosionRadius;
        public float ExplosionRadius => explosionRadius;

        protected override void Init()
        {
            base.Init();
        }

        public override void TearDown()
        {
            Destroy(gameObject);
        }

    }

}
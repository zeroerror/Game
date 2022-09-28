using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class GrenadeEntity : BulletEntity
    {

        public float ExplosionRadius { get; private set; }

        protected override void Init()
        {
            base.Init();
            ExplosionRadius = 7f;
            moveComponent.isPersistentMove = false;
            moveComponent.isPersistentMove = true;
        }

        public override void TearDown()
        {
            Destroy(gameObject);
            Debug.Log($"手雷爆炸！！！");
        }


    }

}
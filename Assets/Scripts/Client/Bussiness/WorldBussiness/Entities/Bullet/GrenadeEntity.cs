using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class GrenadeEntity : BulletEntity
    {

        public float ExplosionRadius { get; private set; }

        protected override void Init()
        {
            base.Init();
            SetLifeTime(3f);
            ExplosionRadius = 7f;
            moveComponent.isPersistentMove = false;
            moveComponent.SetSpeed(10f);
            moveComponent.SetGravity(5f);
            moveComponent.isPersistentMove = true;
        }

        public override void TearDown()
        {
            Destroy(gameObject);
            Debug.Log($"手雷爆炸！！！");
        }


    }

}
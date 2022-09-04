using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class GrenadeEntity : BulletEntity
    {

        protected override void Init()
        {
            base.Init();
            SetLifeTime(3f);
            moveComponent.isPersistentMove = false;
            moveComponent.SetSpeed(10f);
            moveComponent.SetGravity(5f);
            moveComponent.isPersistentMove = true;
        }

        public override void TearDown()
        {
            Destroy(this.gameObject);
        }
   

    }

}
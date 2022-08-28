using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class GrenadeEntity : BulletEntity
    {

        public void SetMoveComponent(float moveSpeed)
        {
            MoveComponent.SetSpeed(moveSpeed);
        }

        public override void TearDown()
        {
            Destroy(this.gameObject);
        }

    }

}
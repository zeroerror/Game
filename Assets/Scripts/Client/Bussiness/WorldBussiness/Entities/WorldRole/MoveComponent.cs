using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class MoveComponent
    {

        Rigidbody rb;

        public MoveComponent(Rigidbody rb)
        {
            this.rb = rb;
        }

        public void Move(Vector3 velocity)
        {
            rb.velocity += velocity;
        }

    }

}
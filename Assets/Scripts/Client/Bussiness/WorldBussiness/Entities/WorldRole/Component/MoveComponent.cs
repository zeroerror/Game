using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class MoveComponent
    {

        Rigidbody rb;
        public float speed;
        public Vector3 Velocity => rb.velocity;
        
        public MoveComponent(Rigidbody rb)
        {
            this.rb = rb;
            speed = 1f;
        }

        public void Move(Vector3 velocity)
        {
            rb.velocity += velocity*speed;
        }

    }

}
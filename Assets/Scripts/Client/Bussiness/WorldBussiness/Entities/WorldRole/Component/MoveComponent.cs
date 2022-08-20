using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class MoveComponent
    {

        Rigidbody rb;
        public float speed;
        
        public MoveComponent(Rigidbody rb)
        {
            this.rb = rb;
            speed = 0.2f;
        }

        public void Move(Vector3 velocity)
        {
            rb.velocity += velocity;
        }

    }

}
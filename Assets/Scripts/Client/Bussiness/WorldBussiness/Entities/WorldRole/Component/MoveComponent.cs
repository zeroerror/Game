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
            speed = 3f;
        }

        public void Move(Vector3 velocity)
        {
            var addVelocity = velocity * speed;
            Debug.Log($"addVelocity :{addVelocity}");
            rb.velocity = addVelocity;
        }

    }

}
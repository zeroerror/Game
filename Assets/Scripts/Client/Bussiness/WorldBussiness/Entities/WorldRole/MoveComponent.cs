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

        public void Move()
        {
            rb.velocity = new Vector3(0, 0, 5);
        }

    }

}
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class MoveComponent
    {

        // TODO: INJECT
        float speed;
        float jumpVelocity;

        Rigidbody rb;
        public Vector3 Velocity => rb.velocity;
        public void SetVelocity(Vector3 velocity) => rb.velocity = velocity;

        public Vector3 CurPos => rb.position;
        public void SetCurPos(Vector3 curPos) => rb.position = curPos;

        Vector3 lastSyncFramePos;
        public Vector3 LastSyncFramePos => lastSyncFramePos;
        public void UpdateLastSyncFramePos() => lastSyncFramePos = FixPosDecimal(rb.position);

        public Vector3 EulerAngel => rb.rotation.eulerAngles;

        public MoveComponent(Rigidbody rb)
        {
            this.rb = rb;
            speed = 5f;
            jumpVelocity = 5f;
            lastSyncFramePos = FixPosDecimal(rb.position);
        }

        public void Move(Vector3 velocity)
        {
            var addVelocity = velocity * speed;
            addVelocity.y = Velocity.y;
            rb.velocity = addVelocity;
            lastSyncFramePos = FixPosDecimal(rb.position);
        }

        public void Jump()
        {
            var newVelocity = rb.velocity;
            newVelocity.y += jumpVelocity;    // Add Axis Y's Velocity
            rb.velocity = newVelocity;
            lastSyncFramePos = FixPosDecimal(rb.position);
            Debug.Log($"JumpJumpJump======> {rb.velocity}");
        }

        public void FaceTo(Vector3 forward)
        {
            rb.rotation = Quaternion.LookRotation(forward);
        }

        public void SetRotaionEulerAngle(Vector3 eulerAngle)
        {
            rb.rotation = Quaternion.Euler(eulerAngle);
        }

        Vector3 FixPosDecimal(Vector3 pos)
        {
            pos *= 10000;
            int posX = (int)pos.x;
            int posY = (int)pos.y;
            int posZ = (int)pos.z;
            pos.x = posX / 10000f;
            pos.y = posY / 10000f;
            pos.z = posZ / 10000f;
            return pos;
        }

    }

}
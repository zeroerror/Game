using UnityEngine;
using Game.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class MoveComponent
    {

        // TODO: INJECT
        float speed;
        public void SetSpeed(float speed) => this.speed = speed;
        float jumpVelocity;
        public void SetJumpVelocity(float jumpVelocity) => this.jumpVelocity = jumpVelocity;

        Rigidbody rb;

        public Vector3 Velocity => rb.velocity;
        public void SetVelocity(Vector3 velocity) => rb.velocity = velocity;

        public Vector3 CurPos => rb.position;
        public void SetCurPos(Vector3 curPos) => rb.position = curPos;

        Vector3 lastSyncFramePos;
        public Vector3 LastSyncFramePos => lastSyncFramePos;
        public void UpdateLastSyncFramePos() => lastSyncFramePos = rb.position.FixDecimal(4);

        public Vector3 EulerAngel => rb.rotation.eulerAngles;

        public MoveComponent(Rigidbody rb, float speed, float jumpVelocity)
        {
            this.rb = rb;
            this.speed = speed;
            this.jumpVelocity = jumpVelocity;
            lastSyncFramePos = rb.position.FixDecimal(4);
        }

        public void Move(Vector3 velocity)
        {
            velocity = velocity.FixDecimal(2);
            Debug.Log($" Move: {velocity}");
            var addVelocity = velocity * speed;
            addVelocity.y = Velocity.y;
            rb.velocity = addVelocity;
            lastSyncFramePos = rb.position.FixDecimal(4);
        }

        public void AddVelocity(Vector3 addVelocity)
        {
            addVelocity = addVelocity.FixDecimal(2);
            Debug.Log($" AddVelocity: {addVelocity}");
            rb.velocity += addVelocity;
            lastSyncFramePos = rb.position.FixDecimal(4);
        }

        public void Jump()
        {
            Debug.Log("Jump");
            var newVelocity = rb.velocity;
            newVelocity.y = jumpVelocity;    // Add Axis Y's Velocity
            rb.velocity = newVelocity;
            lastSyncFramePos = rb.position.FixDecimal(4);
        }

        public void FaceTo(Vector3 forward)
        {
            rb.rotation = Quaternion.LookRotation(forward);
        }

        public void HitByBullet(BulletEntity bulletEntity)
        {
            var velocity = bulletEntity.MoveComponent.Velocity / 10f;
            Debug.Log($"HitByBullet velocity add:  {velocity}");
            rb.velocity += (velocity);
        }

        public void SetRotaionEulerAngle(Vector3 eulerAngle)
        {
            rb.rotation = Quaternion.Euler(eulerAngle);
        }

        public void Reset()
        {
            rb.position = new Vector3(0, 10f, 0);
            rb.velocity = Vector3.zero;
            rb.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }

    }

}
using UnityEngine;
using Game.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class MoveComponent
    {

        // TODO: INJECT
        float speed;
        public void SetSpeed(float speed) => this.speed = speed;
        float jumpSpeed;
        public void SetJumpVelocity(float jumpSpeed) => this.jumpSpeed = jumpSpeed;

        float frictionReduce = 100f;
        public void SetFriction(float friction) => this.frictionReduce = friction;

        Rigidbody rb;

        public Vector3 Velocity => rb.velocity;
        public void SetVelocity(Vector3 velocity) => rb.velocity = velocity;

        public bool isPersistentMove;
        Vector3 moveVelocity;

        Vector3 addVelocity;

        float jumpVelocity;

        public bool IsGrouded { get; private set; }

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
            this.jumpSpeed = jumpVelocity;
            lastSyncFramePos = rb.position.FixDecimal(4);
        }

        public void Move(Vector3 dir)
        {
            dir.Normalize();
            dir = dir.FixDecimal(2);
            this.moveVelocity = dir * speed;
            Debug.Log($" Move Velocity {moveVelocity}");
        }

        public void AddVelocity(Vector3 addVelocity)
        {
            this.addVelocity += addVelocity.FixDecimal(2);
            lastSyncFramePos = rb.position.FixDecimal(4);
            Debug.Log($" AddVelocity: {addVelocity}");
        }

        public void Jump()
        {
            Debug.Log("Jump");
            LeaveGround();
            jumpVelocity = jumpSpeed;
            lastSyncFramePos = rb.position.FixDecimal(4);
        }

        public void Tick(float time)
        {
            var vel = moveVelocity + addVelocity;
            vel.y = rb.velocity.y + jumpVelocity;
            rb.velocity = vel;

            if (isPersistentMove)
            {
                Debug.Log("PersistentMove ");
                return;
            }

            moveVelocity = Vector3.zero;
            jumpVelocity = 0;
            if (IsGrouded && (Mathf.Abs(addVelocity.x) > 0.1f || Mathf.Abs(addVelocity.z) > 0.1f))
            {
                var reduceVelocity = addVelocity.normalized;
                reduceVelocity.y = 0;
                addVelocity -= (frictionReduce * reduceVelocity * time);
                if (Mathf.Abs(addVelocity.x) <= 0.1f) addVelocity.x = 0f;
                if (Mathf.Abs(addVelocity.z) <= 0.1f) addVelocity.z = 0f;
                Debug.Log("摩擦力过后 " + addVelocity);
            }
        }

        public void LeaveGround()
        {
            Debug.Log("离开地面");
            IsGrouded = false;
        }

        public void StandGround()
        {
            Debug.Log("接触地面");
            IsGrouded = true;
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
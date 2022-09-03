using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.WorldBussiness.Shot;

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

        // Rigidbody
        Rigidbody rb;
        public Vector3 Velocity => rb.velocity;
        public void SetVelocity(Vector3 velocity) => rb.velocity = velocity;

        public bool isPersistentMove;
        Vector3 moveVelocity;
        public void SetFrameMoveDir(Vector3 dir)
        {
            dir.Normalize();
            dir = dir.FixDecimal(2);
            this.moveVelocity = dir * speed;
        }
        Vector3 addVelocity;
        public void AddVelocity(Vector3 addVelocity) => this.addVelocity += addVelocity.FixDecimal(2);

        // 重力
        float _gravityVelocity;
        float _gravity;

        float jumpVelocity;
        public float JumpVelocity => jumpVelocity;
        
        public bool IsGrouded { get; private set; }

        public Vector3 CurPos => rb.position;
        public void SetCurPos(Vector3 curPos) => rb.position = curPos;

        public Vector3 EulerAngel => rb.rotation.eulerAngles;

        public MoveComponent(Rigidbody rb, float speed, float jumpVelocity)
        {
            this.rb = rb;
            this.speed = speed;
            this.jumpSpeed = jumpVelocity;
            rb.useGravity = false;  //关闭自动重力
            _gravity = 10;
        }

        public MoveComponentShot ToShot()
        {
            var shot = new MoveComponentShot { CurPos = CurPos, Velocity = Velocity }; ;
            Debug.Log($"MoveComponentShot : CurPos {CurPos}");
            return shot;
        }

        public void Sync(MoveComponentShot moveComponentShot)
        {
            SetCurPos(moveComponentShot.CurPos);
            SetVelocity(moveComponentShot.Velocity);
        }

        public void SetJumpVelocity()
        {
            Debug.Log("SetJumpVelocity");
            if (!IsGrouded) LeaveGround();

            var v = rb.velocity;
            v.y = 0;
            rb.velocity = v;

            jumpVelocity = jumpSpeed;

            _gravityVelocity = 0;
        }

        public void Tick(float fixedDeltaTime)
        {
            var vel = moveVelocity + addVelocity;
            vel.y = rb.velocity.y + jumpVelocity + _gravityVelocity * fixedDeltaTime;
            rb.velocity = vel;

            if (isPersistentMove)
            {
                return;
            }

            //模拟摩擦力
            if (IsGrouded && (Mathf.Abs(addVelocity.x) > 0.1f || Mathf.Abs(addVelocity.z) > 0.1f))
            {
                var reduceVelocity = addVelocity.normalized;
                reduceVelocity.y = 0;
                addVelocity -= (frictionReduce * reduceVelocity * fixedDeltaTime);
                if (Mathf.Abs(addVelocity.x) <= 0.1f) addVelocity.x = 0f;
                if (Mathf.Abs(addVelocity.z) <= 0.1f) addVelocity.z = 0f;
                Debug.Log("摩擦力过后 " + addVelocity);
            }

            //模拟重力
            if (!IsGrouded)
            {
                _gravityVelocity -= (fixedDeltaTime * _gravity);
            }

            // 重置
            moveVelocity = Vector3.zero;
            jumpVelocity = 0;
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

            //重力速度和Y速度归零
            _gravityVelocity = 0;
            var v = rb.velocity;
            v.y = 0;
            rb.velocity = v;
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
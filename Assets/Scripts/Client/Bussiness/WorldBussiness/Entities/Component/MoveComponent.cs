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
        public Vector3 MoveVelocity => moveVelocity;
        public void SetMoveVelocity(Vector3 moveVelocity) => this.moveVelocity = moveVelocity;
        public void AddMoveVelocity(Vector3 dir)
        {
            dir.Normalize();
            dir = dir.FixDecimal(2);
            this.moveVelocity = dir * speed;
        }

        Vector3 extraVelocity;
        public Vector3 ExtraVelcoty => extraVelocity;
        public void SetExtraVelocity(Vector3 extraVelocity) => this.extraVelocity = extraVelocity;
        public void AddExtraVelocity(Vector3 addVelocity) => this.extraVelocity += addVelocity.FixDecimal(2);

        // 重力
        float _gravityVelocity;
        float _gravity;
        public void SetGravity(float _gravity) => this._gravity = _gravity;

        float jumpVelocity;
        public float JumpVelocity => jumpVelocity;

        public bool IsGrouded { get; private set; }
        public bool IsHitWall { get; private set; }

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
            if (fixedDeltaTime == 0) return;

            var vel = moveVelocity + extraVelocity;
            vel.y = rb.velocity.y + jumpVelocity + _gravityVelocity * fixedDeltaTime;
            rb.velocity = vel;
            if (isPersistentMove)
            {
                return;
            }

            //模拟摩擦力
            if (IsGrouded && (Mathf.Abs(extraVelocity.x) > 0.1f || Mathf.Abs(extraVelocity.z) > 0.1f))
            {
                var reduceVelocity = extraVelocity.normalized;
                reduceVelocity.y = 0;
                extraVelocity -= (frictionReduce * reduceVelocity * fixedDeltaTime);
                if (Mathf.Abs(extraVelocity.x) <= 0.1f) extraVelocity.x = 0f;
                if (Mathf.Abs(extraVelocity.z) <= 0.1f) extraVelocity.z = 0f;
                // Debug.Log($"摩擦力过后frictionReduce:{frictionReduce} reduceVelocity:{reduceVelocity}  {addVelocity}" + addVelocity);
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

        public void LeaveWall()
        {
            Debug.Log("离开墙体");
            IsHitWall = false;
        }

        public void HitWall()
        {
            Debug.Log("接触墙体");
            IsHitWall = true;

            // TODO: 惯性指定方向清零
            // if (Velocity != Vector3.zero)
            // {
            // }

            extraVelocity = Vector3.zero;
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
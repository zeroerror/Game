using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.BattleBussiness.Shot;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public class MoveComponent
    {

        #region [静态变量]

        [SerializeField]
        [Header("移动速度")]
        float moveSpeed;

        [SerializeField]
        [Header("持续移动")]
        bool isPersistentMove;
        public void SetPersistentMove(bool flag) => isPersistentMove = flag;

        [SerializeField]
        [Header("地球引力加速度")]
        float gravity;

        [SerializeField]
        [Header("前滚翻速度")]
        float rollSpeed;
        public float RollVelocity => rollSpeed;

        #endregion

        float maximumSpeed = float.MaxValue;
        public void SetMaximumSpeed(float speed) => this.maximumSpeed = speed;

        // TODO: 滑铲摩擦力
        [SerializeField]
        [Header("摩擦力")]
        float frictionReduce = 10f;
        public void SetFriction(float friction) => this.frictionReduce = friction;

        // Rigidbody
        Rigidbody rb;

        public void SetForward(Vector3 forward)
        {
            this.rb.rotation = Quaternion.LookRotation(forward);
        }

        public Vector3 Velocity => rb.velocity;
        public void SetVelocity(Vector3 velocity) => rb.velocity = velocity;

        #region [动态速度]

        // == 移动速度
        [SerializeField]
        [Header("移动速度")]
        Vector3 moveVelocity;
        public Vector3 MoveVelocity => moveVelocity;
        public void SetMoveVelocity(Vector3 moveVelocity) => this.moveVelocity = moveVelocity;
        public void ActivateMoveVelocity(Vector3 dir)
        {
            dir.Normalize();
            dir = dir.FixDecimal(2);
            this.moveVelocity = dir * moveSpeed;
            // Debug.Log($"移动:{moveVelocity}");
        }

        // == 重力速度
        [SerializeField]
        [Header("重力速度")]
        float gravityVelocity;
        public float GravityVelocity => gravityVelocity;
        public void SetGravityVelocity(float gravityVelocity) => this.gravityVelocity = gravityVelocity;

        // == 额外速度
        [SerializeField]
        [Header("额外速度")]
        Vector3 extraVelocity;
        public Vector3 ExtraVelocity => extraVelocity;
        public void SetExtraVelocity(Vector3 extraVelocity) => this.extraVelocity = extraVelocity;
        public void AddExtraVelocity(Vector3 addVelocity)
        {
            Debug.Log($"AddExtraVelocity :{addVelocity}");
            this.extraVelocity += addVelocity.FixDecimal(4);
        }

        #endregion

        // == 碰撞状态
        [SerializeField]
        [Header("是否接触地面")]
        bool isGrounded;
        public bool IsGrounded => isGrounded;
        public void SetIsGrounded(bool flag) => this.isGrounded = flag;

        [SerializeField]
        [Header("是否接触墙面")]
        bool isHitWall;
        public bool IsHitWall => isHitWall;
        public void SetIsHitWall(bool flag) => this.isHitWall = flag;

        public Vector3 Position => rb.position;
        public void SetCurPos(Vector3 curPos) => rb.position = curPos;

        // == Rotation
        public Quaternion Rotation => rb.rotation;
        public Vector3 EulerAngle => rb.rotation.eulerAngles;
        public Vector3 OldEulerAngle { get; private set; }
        public void FaceTo(Vector3 forward)
        {
            var newRotation = Quaternion.LookRotation(forward);
            rb.rotation = newRotation;
        }
        public void SetEulerAngle(Vector3 eulerAngle) => rb.rotation = Quaternion.Euler(eulerAngle);
        public void AddEulerAngleY(float eulerAngleY)
        {
            var euler = rb.rotation.eulerAngles;
            euler.y += eulerAngleY;    //左右看
            rb.rotation = Quaternion.Euler(euler);
        }
        public void SetEulerAngleY(Vector3 eulerAngle)
        {
            var euler = rb.rotation.eulerAngles;
            euler.y = eulerAngle.y;
            rb.rotation = Quaternion.Euler(euler);
        }
        public void FlushEulerAngle() => OldEulerAngle = EulerAngle;
        public bool IsEulerAngleNeedFlush()
        {
            if (Mathf.Abs(OldEulerAngle.x - EulerAngle.x) > 10) return true;
            if (Mathf.Abs(OldEulerAngle.y - EulerAngle.y) > 10) return true;
            if (Mathf.Abs(OldEulerAngle.z - EulerAngle.z) > 10) return true;
            return false;
        }

        public void Inject(Rigidbody rb)
        {
            if (rb == null) return;
            this.rb = rb;
            rb.useGravity = false;  //关闭地球引力
        }

        public bool TryRoll(Vector3 dir)
        {
            if (!IsGrounded) return false;

            dir.Normalize();
            var addVelocity = dir * rollSpeed;
            addVelocity.y = 2f;
            AddExtraVelocity(addVelocity);
            Debug.Log($"前滚翻 dir {dir} rollSpeed {rollSpeed} addVelocity:{addVelocity}");
            return true;
        }

        public void Tick_Rigidbody(float fixedDeltaTime)
        {
            if (fixedDeltaTime == 0) return;

            Vector3 vel = Vector3.zero;
            if (isPersistentMove)
            {
                //比如子弹
                rb.velocity = moveVelocity;
                return;
            }

            // 移动速度可以抵消extraVelocity
            EraseVelocity(ref extraVelocity, moveVelocity);

            vel = moveVelocity + extraVelocity;
            vel.y = rb.velocity.y + extraVelocity.y + gravityVelocity * fixedDeltaTime;   //Y轴
            rb.velocity = vel;

            //限制'最大速度'
            if (rb.velocity.magnitude > maximumSpeed) rb.velocity = rb.velocity.normalized * 30f;

            // 重置 ‘一次性速度’
            moveVelocity = Vector3.zero;
        }

        public void Tick_Friction(float fixedDeltaTime)
        {
            if (!IsGrounded)
            {
                return;
            }

            if (extraVelocity.x == 0 && extraVelocity.z == 0)
            {
                // Debug.Log($"摩擦力不作用 extraVelocity.x == 0 && extraVelocity.z == 0");
                return;
            }

            //模拟摩擦力
            var extraVelocity2DDir = extraVelocity;
            extraVelocity2DDir.y = 0;
            extraVelocity2DDir.Normalize();
            var reduceVelocity = extraVelocity2DDir;
            reduceVelocity *= frictionReduce;

            extraVelocity -= reduceVelocity * fixedDeltaTime;

            var afterExtraVelocity2DDir = extraVelocity;
            afterExtraVelocity2DDir.y = 0;
            afterExtraVelocity2DDir.Normalize();

            if (IsOppositeDir(extraVelocity2DDir, afterExtraVelocity2DDir))
            {
                extraVelocity.x = 0f;
                extraVelocity.z = 0f;
                Debug.Log($"摩擦力作用结束");
                return;
            }

            Debug.Log($"摩擦力----------------------- reduceVelocity {reduceVelocity} extraVelocity {extraVelocity}");

        }

        public void Tick_Gravity(float fixedDeltaTime)
        {
            //模拟重力
            if (!IsGrounded)
            {
                if (extraVelocity.y > 0)
                {
                    gravityVelocity = 0;

                    extraVelocity.y -= (fixedDeltaTime * gravity);
                    if (extraVelocity.y < 0)
                    {
                        extraVelocity.y = 0;
                    }
                }

                gravityVelocity -= (fixedDeltaTime * gravity);
                if (gravityVelocity > 100)
                {
                    gravityVelocity = 100;
                }
            }
            else
            {
                gravityVelocity = 0;
            }
        }

        public void Reset()
        {
            LeaveGround();
            gravityVelocity = 0;
            extraVelocity = Vector3.zero;
            moveVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;

            rb.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }

        public string ToInfo()
        {
            return $"位置: {Position} 移动速度: {MoveVelocity} 跳跃速度: {RollVelocity} 重力速度: {GravityVelocity} 额外速度: {ExtraVelocity} ";
        }

        #region [ Physics Collision]

        public void MoveHitErase(Vector3 hitDir)
        {
            var a = MoveVelocity.normalized;
            var b = hitDir.normalized;
            var cosValue = Vector3.Dot(a, b);
            var reduceVelocity = MoveVelocity * cosValue;
            moveVelocity -= reduceVelocity;
            if (cosValue != 0)
            {
                DebugExtensions.LogWithColor($"贴墙移动，消除垂直墙面速度 {reduceVelocity}  moveVelocity{moveVelocity} cosValue:{cosValue}", "#48D1CC");
            }
        }

        public void HitSomething(Vector3 hitDir)
        {
            // DebugExtensions.LogWithColor($"碰撞某物，碰撞方向:{hitDir}", "#48D1CC");
            //  消除反方向Velocity
            // EraseVelocity(hitDir);   // 存在BUG!!!!!
        }

        public void LeaveSomthing(Vector3 leaveDir)
        {
            // DebugExtensions.LogWithColor($"离开某物，方向:{leaveDir}", "#48D1CC");
        }

        public void EraseVelocity(Vector3 dir)
        {
            dir.Normalize();

            Vector3 a;
            float cosValue;
            Vector3 reduceVelocity;

            var grav = new Vector3(0, this.gravityVelocity, 0);
            a = grav.normalized;
            cosValue = Vector3.Dot(a, dir);
            reduceVelocity = grav * cosValue;
            grav -= reduceVelocity;
            gravityVelocity = grav.y;
            if (gravityVelocity > 0)
            {
                gravityVelocity = 0;
            }
            // DebugExtensions.LogWithColor($"碰撞消除'重力速度':{reduceVelocity}", "#48D1CC");

            a = rb.velocity.normalized;
            cosValue = Vector3.Dot(a, dir);
            reduceVelocity = rb.velocity * cosValue;
            rb.velocity -= reduceVelocity;
            // DebugExtensions.LogWithColor($"碰撞消除'Rigidbody速度':{reduceVelocity}---->新'Rigidbody速度':{rb.velocity}", "#48D1CC");

            a = extraVelocity.normalized;
            cosValue = Vector3.Dot(a, dir);
            reduceVelocity = extraVelocity * cosValue;
            extraVelocity -= reduceVelocity;
            // DebugExtensions.LogWithColor($"碰撞消除'额外速度':{reduceVelocity}---->新'额外速度':{extraVelocity}", "#48D1CC");
        }

        public void JumpboardSpeedUp()
        {
            DebugExtensions.LogWithColor($"跳板起飞！！！！！", "#48D1CC");
            extraVelocity.y += 2f;
            var addVelocity = rb.velocity;
            addVelocity.y = 0;
            addVelocity = addVelocity * 5f;
            AddExtraVelocity(addVelocity);
            DebugExtensions.LogWithColor($"跳板起飞  addVelocity:{addVelocity} extraVelocity:{extraVelocity}", "#48D1CC");
        }

        public void EnterGound()
        {
            if (isGrounded) return;

            DebugExtensions.LogWithColor($"{rb.gameObject.name}接触地面------------------------", "#48D1CC");
            isGrounded = true;
        }

        public void LeaveGround()
        {
            if (!isGrounded) return;

            DebugExtensions.LogWithColor($"{rb.gameObject.name}离开地面-----------------------", "#48D1CC");
            isGrounded = false;
        }

        public void EnterWall()
        {
            if (isHitWall) return;

            DebugExtensions.LogWithColor($"{rb.gameObject.name}接触墙体---------------------------", "#48D1CC");
            isHitWall = true;
        }

        public void LeaveWall()
        {
            if (!isHitWall) return;

            DebugExtensions.LogWithColor($"{rb.gameObject.name}离开墙体-----------------------------", "#48D1CC");
            isHitWall = false;
        }

        #endregion

        #region [Private Func]

        bool IsOppositeDir(Vector3 dir1, Vector3 dir2)
        {
            dir1.Normalize();
            dir2.Normalize();
            var cosVal = Vector3.Dot(dir1, dir2);
            return cosVal < 0;
        }

        void EraseVelocity(ref Vector3 velocity, Vector3 v)
        {
            var cosVal = Vector3.Dot(velocity.normalized, v.normalized);
            if (cosVal >= 0)
            {
                return;
            }

            var reduce = v * cosVal;
            var after = velocity + reduce;
            if (!IsOppositeDir(reduce, after))
            {
                reduce = velocity * cosVal;
                after = velocity + reduce;
            }

            velocity = after;

            Debug.Log($"EraseVelocityByDir  cosVal:{cosVal} reduce:{reduce} ");
        }

        #endregion
    }

}
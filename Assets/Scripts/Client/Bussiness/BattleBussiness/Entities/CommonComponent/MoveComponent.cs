using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.BattleBussiness.Shot;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public class MoveComponent
    {

        #region [基础]

        Rigidbody rb;

        // == Position
        public Vector3 Position => rb.position;
        public void SetPosition(Vector3 curPos) => rb.position = curPos;

        // == Rotation
        public Quaternion Rotation => rb.rotation;
        public void SetRotation(Vector3 eulerAngle) => rb.rotation = Quaternion.Euler(eulerAngle);

        public Vector3 OldEulerAngles { get; private set; }

        // == Velocity
        public Vector3 Velocity => rb.velocity;
        public void SetVelocity(Vector3 velocity) => rb.velocity = velocity;

        float maximumVelocity = float.MaxValue;
        public void SetMaximumVelocity(float velocity) => this.maximumVelocity = velocity;

        public void Inject(Rigidbody rb)
        {
            if (rb == null) return;
            this.rb = rb;
            rb.useGravity = false;  //关闭地球引力
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

        #endregion

        #region [序列化变量]

        [SerializeField]
        [Header("基础移动速度")]
        float basicMoveSpeed;

        [SerializeField]
        [Header("当前移动速度")]
        Vector3 moveVelocity;
        public Vector3 MoveVelocity => moveVelocity;
        public void SetMoveVelocity(Vector3 moveVelocity) => this.moveVelocity = moveVelocity;

        [SerializeField]
        [Header("持续移动")]
        bool isPersistentMove;
        public void SetPersistentMove(bool flag) => isPersistentMove = flag;

        [SerializeField]
        [Header("所受重力")]
        float gravity;

        // == 重力速度
        [SerializeField]
        [Header("当前重力速度")]
        float gravityVelocity;
        public float GravityVelocity => gravityVelocity;
        public void SetGravityVelocity(float gravityVelocity) => this.gravityVelocity = gravityVelocity;

        [SerializeField]
        [Header("额外速度")]
        Vector3 extraVelocity;
        public Vector3 ExtraVelocity => extraVelocity;
        public void SetExtraVelocity(Vector3 extraVelocity) => this.extraVelocity = extraVelocity;

        [SerializeField]
        [Header("摩擦力")]
        float frictionReduce = 10f;

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

        #endregion

        #region [速度]

        public void ActivateMoveVelocity(Vector3 dir)
        {
            dir.Normalize();
            dir = dir.FixDecimal(2);
            this.moveVelocity = dir * basicMoveSpeed;
            // Debug.Log($"移动:{moveVelocity}");
        }

        public void AddExtraVelocity(Vector3 addVelocity)
        {
            Debug.Log($"AddExtraVelocity :{addVelocity}");
            this.extraVelocity += addVelocity.FixDecimal(4);
        }

        #endregion

        #region [旋转]

        public void FaceTo(Vector3 forward)
        {
            var newRotation = Quaternion.LookRotation(forward);
            rb.rotation = newRotation;
        }

        public void AddEulerAngleY(float eulerAngleY)
        {
            var euler = GetEulerAngles();
            euler.y += eulerAngleY;    //左右看
            rb.rotation = Quaternion.Euler(euler);
        }

        public void SetEulerAngleY(Vector3 eulerAngle)
        {
            var euler = rb.rotation.eulerAngles;
            euler.y = eulerAngle.y;
            rb.rotation = Quaternion.Euler(euler);
        }

        public Vector3 GetFaceDir()
        {
            var dir = rb.rotation * Vector3.forward;
            return dir;
        }

        public bool IsRotationNeedFlush()
        {
            var eulerAngles = GetEulerAngles();
            if (Mathf.Abs(OldEulerAngles.x - eulerAngles.x) > 10) return true;
            if (Mathf.Abs(OldEulerAngles.y - eulerAngles.y) > 10) return true;
            if (Mathf.Abs(OldEulerAngles.z - eulerAngles.z) > 10) return true;
            return false;
        }

        public void FlushRotation()
        {
            OldEulerAngles = GetEulerAngles();
        }

        Vector3 GetEulerAngles()
        {
            return rb.rotation.eulerAngles;
        }

        #endregion

        #region  [状态刷新]

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
            if (rb.velocity.magnitude > maximumVelocity) rb.velocity = rb.velocity.normalized * 30f;

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

            if (reduceVelocity == Vector3.zero)
            {
                return;
            }

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

            // Debug.Log($"摩擦力----------------------- reduceVelocity {reduceVelocity} extraVelocity {extraVelocity}");
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

        #endregion

        #region [物理碰撞]

        public void MoveHitErase(Vector3 hitDir)
        {
            EraseVelocity(ref moveVelocity, -hitDir);
        }

        public void HitSomething(Vector3 hitDir)
        {
            // DebugExtensions.LogWithColor($"碰撞某物，碰撞方向:{hitDir}", "#48D1CC");
            //  消除反方向Velocity
            // EraseVelocity(hitDir);  
        }

        // 尚未测试过 可能存在BUG
        public void EraseVelocity(ref Vector3 velocity, Vector3 eraseV)
        {
            var cosVal = Vector3.Dot(velocity.normalized, eraseV.normalized);
            if (cosVal >= 0)
            {
                return;
            }

            var reduce = eraseV * cosVal;
            var after = velocity + reduce;
            if (!IsOppositeDir(reduce, after))
            {
                reduce = velocity * cosVal;
                after = velocity + reduce;
            }

            velocity = after;

            Debug.Log($"EraseVelocityByDir  cosVal:{cosVal} reduce:{reduce} ");
        }


        public void LeaveSomthing(Vector3 leaveDir)
        {
            // DebugExtensions.LogWithColor($"离开某物，方向:{leaveDir}", "#48D1CC");
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

        bool IsOppositeDir(Vector3 dir1, Vector3 dir2)
        {
            dir1.Normalize();
            dir2.Normalize();
            var cosVal = Vector3.Dot(dir1, dir2);
            return cosVal < 0;
        }

    }

}
using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.Interfaces;

namespace Game.Client.Bussiness.WorldBussiness
{

    public enum RoleState
    {
        Normal,
        Move,
        Jump,
        Hooking       //-使用爪钩中
    }

    public class WorldRoleEntity : MonoBehaviour, ICameraTrackObj
    {

        [SerializeField]
        Transform camTrackingObj;
        public Transform CamTrackingObj => camTrackingObj;

        // == ICameraTrackObj
        public Vector3 selfPos => transform.position;
        public Transform camTrackObjTrans => camTrackingObj.transform;

        byte wRid;
        public byte WRid => wRid;
        public void SetWRid(byte wRid) => this.wRid = wRid;

        int connId;
        public int ConnId => connId;
        public void SetConnId(int connId) => this.connId = connId;

        Vector3 offset;
        Vector3 shootPointPos => MoveComponent.CurPos + transform.forward + offset;
        public Vector3 ShootPointPos => shootPointPos.FixDecimal(4);

        // == Component ==
        public MoveComponent MoveComponent { get; private set; }
        public HealthComponent HealthComponent { get; private set; }
        public AnimatorComponent AnimatorComponent { get; private set; }

        // == Rotation ==
        public Vector3 EulerAngle => transform.rotation.eulerAngles;
        public Vector3 OldEulerAngle { get; private set; }
        public void FlushEulerAngle() => OldEulerAngle = EulerAngle;
        public bool IsEulerAngleNeedFlush()
        {
            if (Mathf.Abs(OldEulerAngle.x - EulerAngle.x) > 10) return true;
            if (Mathf.Abs(OldEulerAngle.y - EulerAngle.y) > 10) return true;
            if (Mathf.Abs(OldEulerAngle.z - EulerAngle.z) > 10) return true;
            return false;
        }


        public RoleState RoleState { get; private set; }
        public void SetRoleState(RoleState roleStatus) => this.RoleState = roleStatus;

        public bool IsDead { get; private set; }
        public bool IsOldState;

        public void Awake()
        {
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>(), 5f, 5f);
            AnimatorComponent = new AnimatorComponent(transform.GetComponentInParent<Animator>());
            HealthComponent = new HealthComponent(100f);
            RoleState = RoleState.Normal;
            offset = new Vector3(0, 1f, 0);
        }

        public bool IsIdle()
        {
            return MoveComponent.Velocity == Vector3.zero;
        }

        public void TearDown()
        {
            IsDead = true;
        }

        public void Reborn()
        {
            Debug.Log($"重生 wRid:{wRid}");
            MoveComponent.Reset();
            HealthComponent.Reset();
            IsDead = false;
        }

        // Unity Physics TODO:转移到Tick事件处理
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Field"))
            {
                MoveComponent.EnterGround();
                AnimatorComponent.PlayIdle();
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                MoveComponent.EnterWall();
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Field"))
            {
                MoveComponent.LeaveGround();
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                MoveComponent.LeaveWall();
            }
        }

    }

}
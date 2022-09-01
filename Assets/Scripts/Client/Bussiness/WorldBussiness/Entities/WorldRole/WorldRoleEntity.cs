using UnityEngine;
using Game.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{

    public enum RoleState
    {
        Idle,
        Move,
        Jump,
        Be_Imprisoned       //-被禁锢
    }

    public class WorldRoleEntity : MonoBehaviour
    {

        byte wRid;
        public byte WRid => wRid;
        public void SetWRid(byte wRid) => this.wRid = wRid;

        int connId;
        public int ConnId => connId;
        public void SetConnId(int connId) => this.connId = connId;

        Vector3 offset;
        Vector3 shootPointPos => MoveComponent.CurPos + transform.forward + offset;
        public Vector3 ShootPointPos => shootPointPos.FixDecimal(4);

        public MoveComponent MoveComponent { get; private set; }
        public HealthComponent HealthComponent { get; private set; }
        public AnimatorComponent AnimatorComponent { get; private set; }

        public RoleState RoleState { get; private set; }
        public void SetRoleStatus(RoleState roleStatus) => this.RoleState = roleStatus;

        public RoleState OldRoleState { get; private set; }
        public void UpdateRoleStatus() => this.OldRoleState = RoleState;

        public int stateAhead;

        public bool IsDead { get; private set; }
        public bool IsOldState;

        [SerializeField]
        Transform camTrackingObj;
        public Transform CamTrackingObj => camTrackingObj;

        public void Awake()
        {
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>(), 5f, 5f);
            AnimatorComponent = new AnimatorComponent(transform.GetComponentInParent<Animator>());
            HealthComponent = new HealthComponent(100f);
            RoleState = RoleState.Idle;
            offset = new Vector3(0, 1f, 0);
        }

        public bool IsStateChange()
        {
            return OldRoleState != RoleState;
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

        // Unity Physics 
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Field"))
            {
                MoveComponent.StandGround();
                MoveComponent.UpdateLastSyncFramePos();
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Field"))
            {
                MoveComponent.LeaveGround();
            }
        }

    }

}
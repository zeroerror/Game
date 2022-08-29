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

        Vector3 offset;
        Vector3 shootPointPos => MoveComponent.CurPos + transform.forward + offset;
        public Vector3 ShootPointPos => shootPointPos.FixDecimal(4);

        public MoveComponent MoveComponent { get; private set; }
        public HealthComponent HealthComponent { get; private set; }
        public AnimatorComponent AnimatorComponent { get; private set; }

        public RoleState RoleStatus { get; private set; }
        public void SetRoleStatus(RoleState roleStatus) => this.RoleStatus = roleStatus;

        public bool IsDead { get; private set; }

        [SerializeField]
        Transform camTrackingObj;
        public Transform CamTrackingObj => camTrackingObj;

        public void Awake()
        {
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>(), 5f, 5f);
            AnimatorComponent = new AnimatorComponent(transform.GetComponentInParent<Animator>());
            HealthComponent = new HealthComponent(100f);
            RoleStatus = RoleState.Idle;
            offset = new Vector3(0, 1f, 0);
        }

        public bool IsStateChange(out RoleState roleNewStatus)
        {
            roleNewStatus = RoleState.Idle;
            // Movement
            bool isMove = MoveComponent.Velocity != Vector3.zero;
            bool isMoveStatus = RoleStatus != RoleState.Idle;
            if ((isMove && !isMoveStatus) || (!isMove && isMoveStatus))
            {
                roleNewStatus = isMove ? RoleState.Move : RoleState.Idle;
                return true;
            }
            // TODO:Hit
            // TODO:JUMP

            return false;
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
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public enum RoleState
    {
        Idle,
        Move,
        Jump
    }

    public class WorldRoleEntity : MonoBehaviour
    {

        byte wRid;
        public byte WRid => wRid;
        public void SetWRid(byte wRid) => this.wRid = wRid;

        Vector3 offset;
        public Vector3 ShootPointPos => MoveComponent.CurPos + transform.forward + offset;

        public MoveComponent MoveComponent { get; private set; }

        public AnimatorComponent AnimatorComponent { get; private set; }

        public RoleState RoleStatus { get; private set; }
        public void SetRoleStatus(RoleState roleStatus) => this.RoleStatus = roleStatus;

        [SerializeField]
        Transform camTrackingObj;
        public Transform CamTrackingObj => camTrackingObj;

        public void Awake()
        {
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>());
            AnimatorComponent = new AnimatorComponent(transform.GetComponentInParent<Animator>());
            RoleStatus = RoleState.Idle;
            offset = new Vector3(0, 1.5f, 0);
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

        public void FaceTo(Vector3 v)
        {
            transform.forward = v;
        }

    }

}
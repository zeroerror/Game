using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public enum RoleState
    {
        Idle,
        Move
    }

    public class WorldRoleEntity : MonoBehaviour
    {

        byte wRid;
        public byte WRid => wRid;
        public void SetWRid(byte wRid) => this.wRid = wRid;

        [SerializeField]
        Transform camTrackingObj;
        public Transform CamTrackingObj => camTrackingObj;

        public MoveComponent MoveComponent { get; private set; }

        public RoleState RoleStatus { get; private set; }
        public void SetRoleStatus(RoleState roleStatus) => this.RoleStatus = roleStatus;

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

        public void UpdateState()
        {

        }

        public void Awake()
        {
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>());
            RoleStatus = RoleState.Idle;
        }

        public void Move(Vector3 v)
        {
            var offset = v * MoveComponent.speed;
            transform.position += offset;
        }

    }

}
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public enum RoleState
    {
        Stand,
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

        public void Awake()
        {
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>());
            RoleStatus = RoleState.Stand;
        }

        public void Move(Vector3 v)
        {
            var offset = v * MoveComponent.speed;
            transform.position += offset;
        }

    }

}
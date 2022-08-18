using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WorldRoleEntity : MonoBehaviour
    {

        byte wRid;
        public byte WRid => wRid;
        public void SetRid(byte wRid) => this.wRid = wRid;

        [SerializeField]
        Transform camTrackingObj;
        public Transform CamTrackingObj => camTrackingObj;


        public MoveComponent MoveComponent { get; private set; }

        public void Awake()
        {
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>());
        }

        public void Move(Vector3 v)
        {
            transform.position += v;
            // transform.position = Vector3.Lerp(transform.position, transform.position + v, 0.5f);
        }

    }

}
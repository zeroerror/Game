using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WorldRoleEntity : MonoBehaviour
    {

        sbyte rid;
        public sbyte Rid => rid;
        public void SetRid(sbyte rid) => this.rid = rid;

        [SerializeField]
        Transform camTrackingObj;
        public Transform CamTrackingObj => camTrackingObj;


        public MoveComponent MoveComponent { get; private set; }

        public void Awake()
        {
            MoveComponent = new MoveComponent(transform.GetComponentInParent<Rigidbody>());
        }

    }

}
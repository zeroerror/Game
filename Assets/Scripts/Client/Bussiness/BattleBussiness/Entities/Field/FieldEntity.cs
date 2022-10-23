using UnityEngine;

namespace Game.Client.Bussiness
{

    public class FieldEntity : MonoBehaviour
    {
        byte entityId;
        public byte EntityId => entityId;
        public void SetEntityId(byte entityId) => this.entityId = entityId;

        public CinemachineComponent CameraComponent { get; private set; }

        public Transform Role_Group_Logic { get; private set; }
        public Transform Role_Group_Renderer { get; private set; }

        [SerializeField]
        Transform bornPoint;
        public Vector3 BornPos => bornPoint.position;

        void Awake()
        {
            CameraComponent = new CinemachineComponent();
            Role_Group_Logic = this.transform.Find("Role_Group_Logic");
            Role_Group_Renderer = this.transform.Find("Role_Group_Renderer");
            Debug.Assert(Role_Group_Logic != null);
            Debug.Assert(Role_Group_Renderer != null);

        }

        public void Ctor()
        {

        }

    }


}
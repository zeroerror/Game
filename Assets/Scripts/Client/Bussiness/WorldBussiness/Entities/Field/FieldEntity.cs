using UnityEngine;

namespace Game.Client.Bussiness
{

    public class FieldEntity : MonoBehaviour
    {
        byte fieldId;
        public byte FieldId => fieldId;
        public void SetFieldId(byte id) => fieldId = id;

        public CinemachineComponent CameraComponent { get; private set; }

        public Transform Role_Group_Logic { get; private set; }
        public Transform Role_Group_Renderer { get; private set; }

        void Awake()
        {
            CameraComponent = new CinemachineComponent();
            Role_Group_Logic = this.transform.Find("Role_Group_Logic");
            Role_Group_Renderer = this.transform.Find("Role_Group_Renderer");
            Debug.Assert(Role_Group_Logic != null);
            Debug.Assert(Role_Group_Renderer != null);
        }

    }


}
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WorldRoleEntity : MonoBehaviour
    {

        [SerializeField]
        Transform camTrackingObj;
        public Transform CamTrackingObj => camTrackingObj;

        public MoveComponent MoveComponent { get; private set; }
        public void SetMoveComponent(MoveComponent component) => this.MoveComponent = component;



    }

}
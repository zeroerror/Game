using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Generic
{
    
    public enum CollisionStatus
    {
        None,
        Enter,
        Stay,
        Exit
    }

    public class CollisionExtra
    {

        public CollisionStatus status;

        public Collision collision;
        public GameObject gameObject;
        public Collider Collider => gameObject != null ? gameObject.GetComponent<Collider>() : null;

        public string layerName;

        public FieldType fieldType;

        public Vector3 hitDir;

    }

}
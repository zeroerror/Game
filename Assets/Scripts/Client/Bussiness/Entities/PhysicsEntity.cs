using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness
{

    public class PhysicsEntity : MonoBehaviour
    {

        public struct ColliderExtra
        {
            public bool isEnter;
            public Collider collider;
        }
        List<ColliderExtra> hitColliderList;

        void Awake()
        {
            hitColliderList = new List<ColliderExtra>();
        }

        public void HitColliderListForeach(Action<ColliderExtra> action)
        {
            hitColliderList.ForEach((collider) =>
            {
                action.Invoke(collider);
            });
        }


        //====== Unity Physics 
        void OnTriggerEnter(Collider collider)
        {
            hitColliderList.Add(new ColliderExtra { isEnter = false, collider = collider });
        }

        void OnTriggerExit(Collider collider)
        {
            var ce = hitColliderList.Find((colliderExtra) => colliderExtra.collider.Equals(collider));
            hitColliderList.Remove(ce);
        }

        void OnCollisionEnter(Collision collision)
        {
            hitColliderList.Add(new ColliderExtra { isEnter = false, collider = collision.collider });
        }

        void OnCollisionExit(Collision collision)
        {
            var ce = hitColliderList.Find((colliderExtra) => colliderExtra.collider.Equals(collision.collider));
            hitColliderList.Remove(ce);
        }

    }


}
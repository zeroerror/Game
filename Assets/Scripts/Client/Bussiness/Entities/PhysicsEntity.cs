using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.Log;

namespace Game.Client.Bussiness
{

    public enum CollisionStatus
    {
        None,
        Enter,
        Stay
    }

    public class ColliderExtra
    {
        public CollisionStatus isEnter;
        public Collider collider;
    }

    public class PhysicsEntity : MonoBehaviour
    {
        List<ColliderExtra> hitColliderList;

        void Awake()
        {
            Debug.Log($"PhysicsEntity Created! {gameObject.name}");
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
            var log = $"碰撞信息:{collider.gameObject.name}";
            Debug.Log($"<color=#191970>{log}</color>");
            if (!Exist(collider)) hitColliderList.Add(new ColliderExtra { isEnter = CollisionStatus.Enter, collider = collider });
        }

        void OnTriggerExit(Collider collider)
        {
            var log = $"碰撞信息:{collider.gameObject.name}";
            Debug.Log($"<color=#191970>{log}</color>");
            hitColliderList.Remove(Find(collider));
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!Exist(collision.collider))
            {
                var log = $"碰撞信息:{collision.gameObject.name}";
                Debug.Log($"<color=#191970>{log}</color>");
                hitColliderList.Add(new ColliderExtra { isEnter = CollisionStatus.Enter, collider = collision.collider });
            }
        }

        void OnCollisionExit(Collision collision)
        {
            var log = $"碰撞信息:{collision.gameObject.name}";
            Debug.Log($"<color=#191970>{log}</color>");
            hitColliderList.Remove(Find(collision.collider));
        }

        ColliderExtra Find(Collider collider)
        {
            return hitColliderList.Find((colliderExtra) => colliderExtra.collider.Equals(collider));
        }

        bool Exist(Collider collider)
        {
            ColliderExtra colliderExtra = hitColliderList.Find((ce) => ce.collider.Equals(collider));
            return colliderExtra != null;
        }

    }


}
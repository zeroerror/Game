using System;
using System.Collections.Generic;
using Game.Generic;
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
            // DebugExtensions.LogWithColor($"碰撞Trriger接触:{collider.gameObject.name}", "#48D1CC");
            if (!Exist(collider)) hitColliderList.Add(new ColliderExtra { isEnter = CollisionStatus.Enter, collider = collider });
        }

        void OnTriggerExit(Collider collider)
        {
            // DebugExtensions.LogWithColor($"碰撞Trriger离开:{collider.gameObject.name}", "#48D1CC");
            hitColliderList.Remove(Find(collider));
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!Exist(collision.collider))
            {
                DebugExtensions.LogWithColor($"Collision接触:{collision.gameObject.name}", "#48D1CC");
                hitColliderList.Add(new ColliderExtra { isEnter = CollisionStatus.Enter, collider = collision.collider });
            }
        }

        void OnCollisionExit(Collision collision)
        {
            // DebugExtensions.LogWithColor($"碰撞Collision离开:{collision.gameObject.name}", "#48D1CC");
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
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
        Stay,
        Exit
    }

    public class CollisionExtra
    {
        public CollisionStatus isEnter;
        public Collision collision;
        public Collider collider;
        public Vector3 lastContactPoint;
    }

    public class PhysicsEntity : MonoBehaviour
    {
        List<CollisionExtra> hitCollisionList;

        void Awake()
        {
            Debug.Log($"PhysicsEntity Created! {gameObject.name}");
            hitCollisionList = new List<CollisionExtra>();
        }

        public void HitColliderListForeach(Action<CollisionExtra> action)
        {
            hitCollisionList.ForEach((collider) =>
            {
                action.Invoke(collider);
            });
        }

        public bool RemoveHitCollider(Collider collider)
        {
            var e = hitCollisionList.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.collision.Equals(collider))
                {
                    break;
                }
            }
            return hitCollisionList.Remove(e.Current);
        }

        public bool RemoveHitCollision(CollisionExtra colliderExtra)
        {
            return hitCollisionList.Remove(colliderExtra);
        }


        //====== Unity Physics 
        void OnTriggerEnter(Collider collider)
        {
            // DebugExtensions.LogWithColor($"Trriger接触:{collider.gameObject.name}", "#48D1CC");
            Collision collision = new Collision();
            if (!Exist(collider)) hitCollisionList.Add(new CollisionExtra { isEnter = CollisionStatus.Enter, collider = collider });
        }

        void OnTriggerExit(Collider collider)
        {
            // DebugExtensions.LogWithColor($"Trriger离开:{collider.transform.parent.name}", "#48D1CC");
            var ce = Find(collider);
            ce.isEnter = CollisionStatus.Exit;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!Exist(collision.collider))
            {
                // DebugExtensions.LogWithColor($"Collision接触:{collision.gameObject.name}", "#48D1CC");
                hitCollisionList.Add(new CollisionExtra { isEnter = CollisionStatus.Enter, collision = collision });
            }
        }

        void OnCollisionExit(Collision collision)
        {
            // DebugExtensions.LogWithColor($"Collision离开:{collision.gameObject.name}", "#48D1CC");
            var ce = Find(collision.collider);
            ce.isEnter = CollisionStatus.Exit;
        }

        CollisionExtra Find(Collider collider)
        {
            return hitCollisionList.Find((colliderExtra) =>
            colliderExtra.collision != null ?
            colliderExtra.collision.collider.Equals(collider)
            : colliderExtra.collider.Equals(collider));
        }

        bool Exist(Collider collider)
        {
            CollisionExtra colliderExtra = hitCollisionList.Find((ce) => ce.collision.Equals(collider));
            return colliderExtra != null;
        }

    }


}
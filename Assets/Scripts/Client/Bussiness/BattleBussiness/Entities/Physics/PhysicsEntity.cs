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

        public Collider colliderForTrigger;
        public Collider Collider => collision != null ? collision.collider : colliderForTrigger;

        public Vector3 lastContactPoint;
    }

    public class  PhysicsEntity : MonoBehaviour
    {
        List<CollisionExtra> hitCollisionList;

        void Awake()
        {
            // Debug.Log($"PhysicsEntity Created! {gameObject.name}");
            hitCollisionList = new List<CollisionExtra>();
        }

        public void HitCollisionExtraListForeach(Action<CollisionExtra> action)
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

        public bool RemoveHitCollisionExtra(CollisionExtra colliderExtra)
        {
            return hitCollisionList.Remove(colliderExtra);
        }


        //====== Unity Physics 
        void OnTriggerEnter(Collider collider)
        {
            if (!Exist(collider))
            {
                hitCollisionList.Add(new CollisionExtra { isEnter = CollisionStatus.Enter, colliderForTrigger = collider });
                DebugExtensions.LogWithColor($"Trigger接触:{collider.name} layer:{LayerMask.LayerToName(collider.gameObject.layer)}", "#48D1CC");
            }
        }

        void OnTriggerExit(Collider collider)
        {
            var ce = Find(collider);
            if (ce != null)
            {
                ce.isEnter = CollisionStatus.Exit;
                // DebugExtensions.LogWithColor($"Trigger离开:{collider.name} layer:{LayerMask.LayerToName(collider.gameObject.layer)}", "#48D1CC");
            }
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
            var ce = Find(collision.collider);
            if (ce != null)
            {
                ce.isEnter = CollisionStatus.Exit;
                // DebugExtensions.LogWithColor($"Collision离开:{collision.gameObject.name}", "#48D1CC");
            }
        }

        CollisionExtra Find(Collider collider)
        {
            return hitCollisionList.Find((colliderExtra) => colliderExtra.Collider.Equals(collider));
        }

        bool Exist(Collider collider)
        {
            CollisionExtra colliderExtra = hitCollisionList.Find((ce) => ce.Collider.Equals(collider));
            return colliderExtra != null;
        }

    }


}
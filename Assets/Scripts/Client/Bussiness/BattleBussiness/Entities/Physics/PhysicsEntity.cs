using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Generic;

namespace Game.Client.Bussiness
{

    public class PhysicsEntity : MonoBehaviour
    {

        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        List<CollisionExtra> hitCollisionList;
        int id = 0;

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
                CollisionExtra ce = new CollisionExtra();
                ce.status = CollisionStatus.Enter;
                ce.gameObject = collider.gameObject;
                ce.layerName = LayerMask.LayerToName(collider.gameObject.layer);
                ce.fieldType = FieldType.Ground;
                hitCollisionList.Add(ce);
                DebugExtensions.LogWithColor($"Trigger接触:{collider.name} layerName:{ce.layerName}", "#48D1CC");
            }
        }

        void OnTriggerExit(Collider collider)
        {
            var ce = Find(collider);
            if (ce != null)
            {
                ce.status = CollisionStatus.Exit;
                DebugExtensions.LogWithColor($"Trigger离开:{collider.name} layer:{LayerMask.LayerToName(collider.gameObject.layer)}", "#48D1CC");
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!Exist(collision.collider))
            {
                Vector3 selfPos = transform.position;
                var closestPoint = collision.collider.bounds.ClosestPoint(selfPos);
                if (closestPoint.MostEqualsY(selfPos))
                {
                    if (collision.contactCount != 0)
                    {
                        closestPoint = collision.GetContact(0).point;
                    }
                    else
                    {
                        Debug.LogWarning("collision.contactCount == 0");
                    }
                }

                Vector3 hitDir = Vector3.zero;
                bool isGround = false;
                if (!closestPoint.MostEqualsY(selfPos))
                {
                    hitDir = (closestPoint - selfPos).normalized;
                    isGround = hitDir.y <= 0;
                }
                else
                {
                    isGround = true;
                }

                FieldType fieldType = FieldType.None;
                string layerName = LayerMask.LayerToName(collision.gameObject.layer);
                if (layerName == "Field")
                {
                    if (isGround)
                    {
                        fieldType = FieldType.Ground;
                    }
                    else
                    {
                        fieldType = FieldType.Wall;
                    }
                }

                CollisionExtra collisionExtra = new CollisionExtra();
                collisionExtra.status = CollisionStatus.Enter;
                collisionExtra.collision = collision;
                collisionExtra.gameObject = collision.gameObject;
                collisionExtra.fieldType = fieldType;
                collisionExtra.layerName = LayerMask.LayerToName(collision.gameObject.layer);
                collisionExtra.hitDir = hitDir;
                hitCollisionList.Add(collisionExtra);
                // DebugExtensions.LogWithColor($"接触:fieldType:{fieldType.ToString()} {collision.gameObject.name} ", "#48D1CC");
            }
        }

        void OnCollisionExit(Collision collision)
        {
            var ce = Find(collision.collider);
            if (ce != null)
            {
                ce.status = CollisionStatus.Exit;
                // DebugExtensions.LogWithColor($"离开:fieldType:{ce.fieldType.ToString()} {collision.gameObject.name}", "#48D1CC");
            }
        }

        CollisionExtra Find(Collider collider)
        {
            return hitCollisionList.Find((colliderExtra) => colliderExtra.gameObject.Equals(collider.gameObject));
        }

        bool Exist(Collider collider)
        {
            CollisionExtra colliderExtra = hitCollisionList.Find((ce) => ce.gameObject.Equals(collider.gameObject));
            return colliderExtra != null;
        }

    }


}
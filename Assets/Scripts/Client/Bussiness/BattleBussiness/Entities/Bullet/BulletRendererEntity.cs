using System.Collections.Generic;
using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BulletRendererEntity : MonoBehaviour
    {
        // ID Info
        int entityID;
        public int EntityID => entityID;
        public void SetEntityID(int v) => entityID = v;

        public void SetPosition(Vector3 v) => transform.position = v;
        public void SetRotation(Quaternion v) => transform.rotation = v;

        public float posAdjust;
        public float rotAdjust;

        public void Ctor()
        {
        }

        public virtual void TearDown()
        {
            Debug.Log($"摧毁子弹Renderer   EntityID:{entityID}");
            Destroy(gameObject);
        }

    }

}
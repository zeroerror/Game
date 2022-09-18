using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.Interfaces;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class WorldRoleRendererEntity : MonoBehaviour, ICameraTrackObj
    {

        byte wRid;
        public byte WRid => wRid;
        public void SetWRid(byte wrid) => this.wRid = wrid;

        Animator animator;
        public AnimatorComponent AnimatorComponent { get; private set; }

        // == ICameraTrackObj ==
        GameObject camTrackingObj;
        public Transform CamTrackObjTrans => camTrackingObj.transform;
        public void SetCamTrackingPos(Vector3 pos) => this.camTrackingObj.transform.position = pos;
        public Vector3 SelfPos => transform.position;

        // == Hang Point ==
        public Transform handPoint;

        public float posAdjust;
        public float rotAdjust;

        public void Ctor()
        {
            animator = GetComponent<Animator>();
            Debug.Assert(animator != null);

            AnimatorComponent = new AnimatorComponent(animator);
            camTrackingObj = new GameObject($"CameraTrackingObject_RID_{wRid}");

            posAdjust = 15f;
            rotAdjust = 15f;
        }
    }

}
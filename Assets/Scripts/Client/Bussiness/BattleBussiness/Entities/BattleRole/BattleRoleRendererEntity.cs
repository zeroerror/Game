using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.Interfaces;
using UnityEngine.UI;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleRoleRendererEntity : MonoBehaviour, ICameraTrackObj
    {

        int entityId;
        public int EntityId => entityId;
        public void SetEntityId(int entityId) => this.entityId = entityId;

        Animator animator;
        public AnimatorComponent AnimatorComponent { get; private set; }
        public float staticTime;

        // == ICameraTrackObj ==
        GameObject camTrackingObj;
        public Transform CamTrackObjTrans => camTrackingObj.transform;
        public void SetCamTrackingPos(Vector3 pos) => this.camTrackingObj.transform.position = pos;
        public Vector3 SelfPos => transform.position;

        // == Hang Point ==
        public Transform handPoint;

        public float posAdjust;
        public float rotAdjust;

        // == UI ==
        Slider bloodSlider;
        public Slider BloodSlider =>bloodSlider;

        public void Ctor()
        {
            animator = GetComponent<Animator>();
            Debug.Assert(animator != null);

            bloodSlider = transform.GetComponentInChildren<Slider>();
            Debug.Assert(bloodSlider != null);

            AnimatorComponent = new AnimatorComponent(animator);
            camTrackingObj = new GameObject($"相机跟随角色物体_RID_{entityId}");

            posAdjust = 15f;
            rotAdjust = 15f;
        }
    }

}
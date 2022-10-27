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

        // == HUD ==
        Slider bloodSlider;
        public Slider BloodSlider => bloodSlider;

        Slider armorSlider;
        public Slider ArmorSlider => armorSlider;

        Transform damageTextTF;
        public Transform[] DamageTextTFArray { get; private set; }

        int curDamageTextIndex;
        public Transform GetDamageTextTF()
        {
            var array = DamageTextTFArray;
            if (curDamageTextIndex > array.Length)
            {
                curDamageTextIndex = curDamageTextIndex % array.Length;
            }

            return array[curDamageTextIndex++];
        }

        public void Ctor()
        {
            CtorHUD();
            AnimatorComponent = new AnimatorComponent(animator);
            camTrackingObj = new GameObject($"相机跟随角色物体_RID_{entityId}");
            posAdjust = 15f;
            rotAdjust = 15f;
        }

        void CtorHUD()
        {
            animator = GetComponent<Animator>();
            Debug.Assert(animator != null);
            bloodSlider = transform.Find("BloodSlider").GetComponent<Slider>();
            Debug.Assert(bloodSlider != null);
            armorSlider = transform.Find("ArmorSlider").GetComponent<Slider>();
            Debug.Assert(armorSlider != null);

            damageTextTF = transform.Find("role_renderer/Root/HUD/DamageText");
            Debug.Assert(damageTextTF != null);

            var childCount = damageTextTF.childCount;
            DamageTextTFArray = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                DamageTextTFArray[i] = damageTextTF.GetChild(i);
            }
        }
    }

}
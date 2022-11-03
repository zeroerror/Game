using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.Interfaces;
using UnityEngine.UI;
using TMPro;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleRoleRendererEntity : MonoBehaviour, ICameraTrackObj
    {
        // ID Info
        int entityID;
        public int EntityID => entityID;
        public void SetEntityID(int v) => entityID = v;

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

        public float posAdjust = 15f;
        public float rotAdjust = 15f;

        // == HUD ==
        Slider bloodSlider;
        public Slider BloodSlider => bloodSlider;

        Slider armorSlider;
        public Slider ArmorSlider => armorSlider;

        Transform damageTextGroupTF;

        Transform[] damageTextTFArray;

        public void Ctor()
        {
            Ctor_Obj();
            Ctor_HUD();
            Ctor_Component();
        }

        void Ctor_Obj()
        {
            camTrackingObj = new GameObject($"相机跟随角色物体_RID_{entityID}");
        }

        void Ctor_HUD()
        {
            bloodSlider = transform.Find("Root/HUD/BloodSlider").GetComponent<Slider>();
            Debug.Assert(bloodSlider != null);
            armorSlider = transform.Find("Root/HUD/ArmorSlider").GetComponent<Slider>();
            Debug.Assert(armorSlider != null);

            damageTextGroupTF = transform.Find("Root/HUD/DamageTextGroup");
            Debug.Assert(damageTextGroupTF != null);

            var childCount = damageTextGroupTF.childCount;
            damageTextTFArray = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                damageTextTFArray[i] = damageTextGroupTF.GetChild(i);
            }
        }

        void Ctor_Component()
        {
            animator = GetComponentInChildren<Animator>();
            Debug.Assert(animator != null);
            AnimatorComponent = new AnimatorComponent(animator);
        }

        public void SetDamageText(string damageText)
        {
            // - Random TextTF
            var array = damageTextTFArray;
            var randomIndex = Random.Range(0, array.Length);
            var damageTextTF = array[randomIndex];
            var text = damageTextTF.GetComponent<TMP_Text>();
            text.text = damageText;
            text.gameObject.SetActive(true);

            // - Random Clip
            var anim = damageTextTF.GetComponent<Animation>();
            var clipCount = anim.GetClipCount();
            var randomClipIndex = Random.Range(0, clipCount);
            int i = 0;
            foreach (AnimationState animationState in anim)
            {
                if (i == randomClipIndex)
                {
                    animationState.normalizedTime = 0;
                    anim.Play(animationState.name);
                    return;
                }
                i++;
            }
        }

    }

}
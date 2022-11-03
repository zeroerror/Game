using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Client.Bussiness.Interfaces;

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

        Dictionary<int, List<AnimationState>> damageTextAnimDic;

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
            Ctor_HUD_Slider();
            Ctor_HUD_DamageText();
        }

        void Ctor_HUD_Slider()
        {
            // - Slider
            bloodSlider = transform.Find("Root/HUD/BloodSlider").GetComponent<Slider>();
            Debug.Assert(bloodSlider != null);
            armorSlider = transform.Find("Root/HUD/ArmorSlider").GetComponent<Slider>();
            Debug.Assert(armorSlider != null);
        }

        void Ctor_HUD_DamageText()
        {
            damageTextGroupTF = transform.Find("Root/HUD/DamageTextGroup");
            Debug.Assert(damageTextGroupTF != null);

            var childCount = damageTextGroupTF.childCount;
            damageTextTFArray = new Transform[childCount];
            damageTextAnimDic = new Dictionary<int, List<AnimationState>>();
            for (int i = 0; i < childCount; i++)
            {
                var damageTextTF = damageTextGroupTF.GetChild(i);
                damageTextTFArray[i] = damageTextTF;

                var anim = damageTextTF.GetComponent<Animation>();
                var list = new List<AnimationState>();
                foreach (AnimationState animationState in anim)
                {
                    list.Add(animationState);
                }
                damageTextAnimDic[i] = list;
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
            var animStateList = damageTextAnimDic[randomIndex];
            var randomIndex2 = Random.Range(0, animStateList.Count);
            var animState = animStateList[randomIndex2];
            var anim = damageTextTF.GetComponent<Animation>();
            anim.Play(animState.name);
            return;
        }

    }

}
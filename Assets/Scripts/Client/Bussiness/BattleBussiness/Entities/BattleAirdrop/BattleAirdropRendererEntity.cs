using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleAirdropRendererEntity : PhysicsEntity
    {

        // ID Info
        int entityID;
        public int EntityID => entityID;
        public void SetEntityID(int v) => entityID = v;

        // == HUD ==
        Slider bloodSlider;
        public Slider BloodSlider => bloodSlider;

        Transform damageTextGroupTF;

        Transform[] damageTextTFArray;

        Dictionary<int, List<AnimationState>> damageTextAnimDic;

        public float posAdjust = 15f;
        public float rotAdjust = 15f;

        public void Ctor()
        {
            Ctor_HUD();
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

        public void TearDown()
        {
            GameObject.Destroy(gameObject);
            GameObject.Destroy(this);
        }

    }

}
using System.Collections;

namespace UnityEngine.UI
{
    public sealed class BloodImage : RawImage
    {

        [SerializeField] RectTransform _fadeImage;

        [SerializeField][Header("褪去时间")] float _fadeTime = 0.2f;

        Slider bloodSlider;
        Rect bloodSliderRect;
        Coroutine _cor;

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            if (bloodSlider == null)
            {
                InitBloodSlider();
            }

            //获取血条  
            var width = bloodSliderRect.width;
            var height = bloodSliderRect.height;
            uvRect = new Rect(0, 0, bloodSlider.maxValue, 1);

            //获取血条的值  
            if (bloodSlider != null && gameObject.activeInHierarchy)
            {
                //刷新血条的显示  
                float value = bloodSlider.value;
                float maxValue = bloodSlider.maxValue;
                float num = (value / maxValue) * width;
                uvRect = new Rect(0, 0, value, 1);
                //刷新渐变
                if (_cor != null) StopCoroutine(_cor);
                Vector2 rt = _fadeImage.offsetMax;
                float targetValue = num - width;
                if (rt.x < targetValue)
                {
                    rt.x = targetValue;
                    _fadeImage.offsetMax = rt;
                }
                else
                {
                    _cor = StartCoroutine(UpdateFadeImage(targetValue));
                }
            }
        }

        void InitBloodSlider()
        {
            bloodSlider = transform.GetComponentInParent<Slider>();
            bloodSliderRect = bloodSlider.GetComponent<RectTransform>().rect;
            Transform fade = bloodSlider.transform.Find("Background/Fade");

            if (fade == null)
            {
                fade = new GameObject("Fade", typeof(Image)).transform;
                fade.SetParent(bloodSlider.transform.Find("Background"));
            }

            _fadeImage = fade.GetComponent<RectTransform>();
            SetRectTransform(ref _fadeImage);
        }

        void SetRectTransform(ref RectTransform rct)
        {
            rct.anchorMin = Vector2.zero;
            rct.anchorMax = Vector2.one;
            rct.offsetMin = Vector2.zero;
            rct.offsetMax = Vector2.zero;
        }

        IEnumerator UpdateFadeImage(float targetValue)
        {
            Vector2 rt = _fadeImage.offsetMax;
            float fadeValue = (rt.x - targetValue) / (_fadeTime / Time.deltaTime);
            while (rt.x > targetValue)
            {
                rt.x -= fadeValue;
                //rt.x = rt.x < targetValue ? targetValue : rt.x;
                _fadeImage.offsetMax = rt;
                yield return null;
            }
        }

    }
}

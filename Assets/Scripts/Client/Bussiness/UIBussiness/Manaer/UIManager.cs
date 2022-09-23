using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ZeroUIFrame;
using Game.Generic;
using Game.Bussiness.UIBussiness;

namespace Game.Bussiness.UIBussiness
{

    public static class UIManager
    {

        public static void Ctor()
        {
            int layer = LayerMask.NameToLayer("UI");
            UIRoot = new GameObject("UICanvas", typeof(Canvas), typeof(CanvasScaler));
            _uiRootCanvas = UIRoot.GetComponent<Canvas>();
            _uiRootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            _uiRootCanvas.sortingLayerID = SortingLayer.NameToID("Billboard");
            UIRoot.layer = layer;
            _uiRootRt = UIRoot.GetComponent<RectTransform>();
            GameObject.DontDestroyOnLoad(UIRoot);
            CanvasScaler tempScale = UIRoot.GetComponent<CanvasScaler>();
            tempScale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            tempScale.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            tempScale.referenceResolution = UIDef.UIResolution;
            _uiRootRt.position = Vector3.zero;//CanvasScaler 会进行重置坐标 等其加载完成

            _uiMainView = new GameObject("UIMainView", typeof(RectTransform)).GetComponent<RectTransform>();
            _SetRectTransform(ref _uiMainView);
            _uiMainView.transform.SetParent(UIRoot.transform, false);

            foreach (var item in SortingLayer.layers)
            {
                if (item.value == 0) continue;
                var layerGO = new GameObject(item.name, typeof(Canvas));
                var layerRct = layerGO.GetComponent<RectTransform>();
                layerRct.transform.SetParent(_uiMainView.transform, false);
                layerRct.gameObject.layer = layer;
                Canvas canvas = layerGO.GetComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingLayerID = item.id;
                canvas.sortingOrder = _curSortingOrder;
                _layerSortingDic.Add(item.name, new int2(_curSortingOrder, 0));
                _curSortingOrder += 10;
                _SetRectTransform(ref layerRct);
                _uiLayerDic.Add(item.name, layerRct.transform);
            }

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule), typeof(BaseInput));
            eventSystem.transform.SetParent(UIRoot.transform, false);
        }

        public static bool IsActive(string uiName)
        {
            Transform ui = null;
            return _CheckUI(uiName, ref ui) && ui.gameObject.activeInHierarchy;
        }

        public static UIBehavior OpenUI(string uiName, params object[] args)
        {
            Transform ui = null;
            if (!_CheckUI(uiName, ref ui))
            {
                if (!_TryCreateUI(uiName, ref ui))
                {
                    Debug.LogError(string.Format("UI: {0} 不存在！", uiName));
                    return null;
                }
                else
                {
                    Debug.Log(new StringBuilder("UI: ").Append(uiName).Append(" Loaded*************"));
                }
            }

            var uIBehavior = ui.GetComponent<UIBehavior>();

            // 传参args
            uIBehavior.args = args;

            if (ui.gameObject.activeInHierarchy) return uIBehavior;

            Canvas canvas;
            if (!ui.GetComponent<Canvas>()) canvas = ui.gameObject.AddComponent<Canvas>();
            else canvas = ui.GetComponent<Canvas>();
            string layer = uiName.Split('_')[0];
            int2 value = _layerSortingDic[layer];

            if (!_uiDic.ContainsKey(uiName))
            {
                _uiDic.Add(uiName, ui);
                canvas.overrideSorting = true;
                canvas.sortingLayerID = SortingLayer.NameToID(layer);
            }
            else
            {
                Debug.Log(new StringBuilder("UI: ").Append(uiName).Append(" ReOpened---"));
            }

            value.y++;
            canvas.sortingOrder = value.x + value.y;
            ui.gameObject.SetActive(true);
            _layerSortingDic[layer] = value;

            return uIBehavior;
        }

        public static GameObject GetUIAsset(string uiAssetName)
        {
            return UIPanelAssets.Get(uiAssetName);
        }

        public static void CloseUI(string uiName)
        {
            Transform ui = null;
            if (!_CheckUI(uiName, ref ui))
            {
                return;
            }
            if (!ui.gameObject.activeInHierarchy)
            {
                return;
            }

            Canvas canvas = ui.GetComponent<Canvas>();
            string layer = uiName.Split('_')[0];
            Transform layerTrans = _uiLayerDic[layer];
            Canvas[] allCanvas = layerTrans.GetComponentsInChildren<Canvas>();

            // Update Layer Sorting
            for (int i = 1; i < allCanvas.Length; i++)
            {
                Canvas otherCanvas = allCanvas[i];
                if (otherCanvas.sortingOrder > canvas.sortingOrder)
                {
                    otherCanvas.sortingOrder--;
                }
            }

            int2 value = _layerSortingDic[layer];
            value.y--;
            _layerSortingDic[layer] = value;
            ui.gameObject.SetActive(false);
        }

        public static void DestoryUI(string uiName)
        {
            Transform ui = null;
            if (!_CheckUI(uiName, ref ui))
            {
                return;
            }
            GameObject.Destroy(ui.gameObject);
            string layer = uiName.Split('_')[0];
            int2 value = _layerSortingDic[layer];
            value.y--;
            _layerSortingDic[layer] = value;
        }

        private static bool _TryCreateUI(string uiName, ref Transform ui)
        {
            GameObject go = UIPanelAssets.Get(uiName);
            if (!go) return false;

            go.SetActive(false);
            go = GameObject.Instantiate(go);
            ui = go.transform;
            ui.name = uiName;

            if (ui.GetComponent<GraphicRaycaster>() == null)
            {
                ui.gameObject.AddComponent<GraphicRaycaster>();
            }

            string layer = uiName.Split('_')[0];
            ui.SetParent(_uiLayerDic[layer], false);
            return true;
        }

        private static bool _CheckUI(string uiName, ref Transform ui)
        {
            if (_uiDic.ContainsKey(uiName))
            {
                ui = _uiDic[uiName];
                return true;
            }

            return false;
        }

        private static void _SetRectTransform(ref RectTransform rct)
        {
            rct.pivot = new Vector2(0.5f, 0.5f);
            rct.anchorMin = Vector2.zero;
            rct.anchorMax = Vector2.one;
            rct.offsetMax = Vector2.zero;
            rct.offsetMin = Vector2.zero;
        }

        public static GameObject UIRoot { get; private set; }
        static Canvas _uiRootCanvas;
        static RectTransform _uiRootRt;
        static GameObject _battleUIRoot;
        static Canvas _battleUIRootCanvas;
        static RectTransform _battleUIRootRt;
        static RectTransform _uiMainView;
        static Dictionary<string, Transform> _uiLayerDic = new Dictionary<string, Transform>();
        static Dictionary<string, Transform> _uiDic = new Dictionary<string, Transform>();
        static int _curSortingOrder = 0;
        static Dictionary<string, int2> _layerSortingDic = new Dictionary<string, int2>();

    }

}


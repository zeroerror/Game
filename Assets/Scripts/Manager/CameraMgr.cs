using UnityEngine;

namespace Game.Manager
{

    public static class CameraManager
    {

        public static Transform UICamTrans;
        public static Transform WorldCamTrans;

        public static void Ctor()
        {
            int layer = LayerMask.NameToLayer("UI");
            UICamTrans = new GameObject("UICamera", typeof(Camera)).transform;
            var uiCamera = UICamTrans.GetComponent<Camera>();
            uiCamera.clearFlags = CameraClearFlags.SolidColor;
            uiCamera.cullingMask = 1 << layer;
            uiCamera.orthographic = true;
            uiCamera.orthographicSize = 1f;
            uiCamera.nearClipPlane = -10;
            uiCamera.farClipPlane = 10;
            uiCamera.allowHDR = false;
            uiCamera.allowMSAA = false;
            uiCamera.allowDynamicResolution = false;
            uiCamera.useOcclusionCulling = false;
        }

        public static void AddCullingMask(string layer)
        {
            WorldCamTrans.GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer(layer));
        }

        public static void RemoveCullingMask(string layer)
        {
            WorldCamTrans.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
        }

    }

}




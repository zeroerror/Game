using UnityEngine;
using Game.Infrastructure.Input;

namespace Game.Client
{

    public static class CameraManager
    {

        public static Transform UICamTrans { get; private set; }

        public static Transform WorldCamTrans { get; private set; }
        public static void SetWorldCamTrans(Transform trans) => WorldCamTrans = trans;

        public static CinemachineExtra CinemachineExtra { get; private set; }
        public static void SetCinemachineExtra(CinemachineExtra cinemachineExtra) => CinemachineExtra = cinemachineExtra;

        public static void Ctor()
        {
            CreateUICamera();
        }

        public static void TearDown()
        {

        }

        public static void AddCullingMask(string layer)
        {
            WorldCamTrans.GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer(layer));
        }

        public static void RemoveCullingMask(string layer)
        {
            WorldCamTrans.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
        }

        static void CreateUICamera()
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

    }

}
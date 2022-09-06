
using System.Collections.Generic;
using Game.Client.Bussiness.Interfaces;
using Game.Infrastructure.Input;
using UnityEngine;

namespace Game.Client.Bussiness
{

    public enum CameraView
    {
        FirstView,
        ThirdView
    }

    public class CinemachineComponent
    {

        // == 第三人称相机
        public CinemachineExtra ThirdViewCam;
        public void SetThirdViewCam(CinemachineExtra cam)
        {
            ThirdViewCam = cam;
            camList.Add(ThirdViewCam);
        }
        public void OpenThirdViewCam(ICameraTrackObj cameraTrackObj)
        {
            Debug.Log("打开第三人称");
            ThirdViewCam.FollowSolo(cameraTrackObj.camTrackObjTrans, 3f);

            SwitchCamView(ThirdViewCam.gameObject.name);
            CurrentCameraView = CameraView.ThirdView;
        }


        // == 第一人称相机
        public CinemachineExtra FirstViewCam;
        public void SetFirstViewCam(CinemachineExtra cam)
        {
            FirstViewCam = cam;
            camList.Add(FirstViewCam);
        }
        public void OpenFirstViewCam(ICameraTrackObj cameraTrackObj)
        {
            Debug.Log("打开第一人称相机");
            FirstViewCam.FollowSolo(cameraTrackObj.camTrackObjTrans);
            FirstViewCam.LookAtSolo(null);

            SwitchCamView(FirstViewCam.gameObject.name);
            CurrentCameraView = CameraView.FirstView;
        }

        // == 当前相机模式
        public CameraView CurrentCameraView { get; private set; }
        public CinemachineExtra CurrentCamera { get; private set; }

        public CinemachineComponent()
        {
            camList = new List<CinemachineExtra>();
        }

        // == 已设置相机缓存
        public List<CinemachineExtra> camList;

        // == Private Func == 
        void SwitchCamView(string name)
        {
            camList.ForEach((cam) =>
            {
                if (cam.gameObject.name == name)
                {
                    cam.gameObject.SetActive(true);
                    CurrentCamera = cam;
                    Debug.Log($"打开相机： {name}");
                    return;
                }

                cam.gameObject.SetActive(false);
            });
        }


    }


}

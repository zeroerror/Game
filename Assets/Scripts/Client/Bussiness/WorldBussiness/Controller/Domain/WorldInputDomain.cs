using Game.Infrastructure.Input;
using UnityEngine;
using ZeroFrame.ZeroMath;

namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class WorldInputDomain
    {

        public void UpdateCameraByCameraView(WorldRoleLogicEntity owner, CameraView cameraView, CinemachineExtra curCam, Vector2 inputAxis)
        {
            if (owner == null) return;

            var roleRenderer = owner.roleRenderer;
            Vector3 trackPos = roleRenderer.transform.position;
            switch (cameraView)
            {
                case CameraView.FirstView:
                    trackPos += roleRenderer.transform.forward * 0.5f;
                    trackPos.y -= 1.2f;

                    curCam.AddEulerAngleX(-inputAxis.y);
                    curCam.AddEulerAngleY(inputAxis.x);
                    owner.MoveComponent.SetEulerAngleY(curCam.EulerAngles);
                    break;
                case CameraView.ThirdView:
                    break;
            }

            roleRenderer.SetCamTrackingPos(trackPos);
        }

        public Vector3 GetMoveDirByCameraView(WorldRoleLogicEntity owner, Vector3 moveAxis, CameraView cameraView)
        {
            Vector3 moveDir = Vector3.zero;
            switch (cameraView)
            {
                case CameraView.FirstView:
                    Vector3 roleForward = owner.transform.forward;
                    roleForward.y = 0;
                    VectorHelper2D.GetRotVector(roleForward.x, roleForward.z, -90, out float rightX, out float rightZ);
                    Vector3 roleRight = new Vector3(rightX, 0, rightZ);
                    moveDir = moveAxis;
                    moveDir.x *= roleForward.x;
                    moveDir = moveAxis.z * roleForward; //前后
                    moveDir += moveAxis.x * roleRight;  //左右
                    break;
                case CameraView.ThirdView:
                    moveDir = moveAxis;
                    break;
            }

            return moveDir.normalized;
        }

        public Vector3 GetShotPointByCameraView(CameraView cameraView, WorldRoleLogicEntity roleLogicEntity)
        {
            var mainCam = Camera.main;
            if (mainCam == null) return Vector3.zero;

            switch (cameraView)
            {
                case CameraView.FirstView:
                    var ray = mainCam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit)) return hit.point;
                    break;
                case CameraView.ThirdView:
                    var roleTrans = roleLogicEntity.MoveComponent.CurPos;
                    var forward = roleLogicEntity.transform.forward;
                    if (Physics.Raycast(roleTrans, forward, out hit, 100f)) return hit.point;
                    break;
            }

            return Vector3.zero;
        }

    }

}
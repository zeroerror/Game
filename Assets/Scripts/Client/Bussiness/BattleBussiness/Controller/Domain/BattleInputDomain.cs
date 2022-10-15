using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Infrastructure.Input;
using UnityEngine;
using ZeroFrame.ZeroMath;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleInputDomain
    {

        BattleFacades battleFacades;

        public BattleInputDomain()
        {

        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }


        public void UpdateCameraByCameraView(Vector2 inputAxis)
        {
            var owner = battleFacades.Repo.RoleRepo.Owner;
            if (owner == null) return;

            var curFieldEntity = battleFacades.Repo.FiledRepo.CurFieldEntity;
            if (curFieldEntity == null) return;

            var cameraComponent = curFieldEntity.CameraComponent;
            var currentCam = cameraComponent.CurrentCamera;
            var cameraView = cameraComponent.CurrentCameraView;

            var roleRenderer = owner.roleRenderer;
            Vector3 trackPos = roleRenderer.transform.position;

            if (cameraView == CameraView.FirstView)
            {
                trackPos += roleRenderer.transform.forward * 0.5f;
                trackPos.y -= 1.2f;

                currentCam.AddEulerAngleX(-inputAxis.y);
                currentCam.AddEulerAngleY(inputAxis.x);
                owner.MoveComponent.SetEulerAngleY(currentCam.EulerAngles);
            }

            roleRenderer.SetCamTrackingPos(trackPos);
        }

        public Vector3 GetMoveDirByCameraView(BattleRoleLogicEntity owner, Vector3 moveAxis, CameraView cameraView)
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

        public Vector3 GetShotPointByCameraView(CameraView cameraView, BattleRoleLogicEntity roleLogicEntity)
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
                    var roleTrans = roleLogicEntity.MoveComponent.Position;
                    var forward = roleLogicEntity.transform.forward;
                    if (Physics.Raycast(roleTrans, forward, out hit, 1000f)) return hit.point;
                    break;
            }

            return Vector3.zero;
        }


    }

}
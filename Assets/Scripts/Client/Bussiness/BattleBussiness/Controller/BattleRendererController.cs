


using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Protocol.Battle;
using System.Collections.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattleRendererController
    {

        BattleFacades battleFacades;
        float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;

        public BattleRendererController()
        {
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick()
        {
        }

        public void Update(float deltaTime)
        {
            Update_RoleRenderer(deltaTime);
            Update_Camera();
        }

        #region [Renderer Update]

        void Update_RoleRenderer(float deltaTime)
        {
            var domain = battleFacades.Domain.RoleDomain;
            domain.Update_RoleRenderer(deltaTime);
        }

        void Update_Camera()
        {
            var curFieldEntity = battleFacades.Repo.FiledRepo.CurFieldEntity;
            if (curFieldEntity == null) return;

            var cameraComponent = curFieldEntity.CameraComponent;
            var currentCam = cameraComponent.CurrentCamera;
            var cameraView = cameraComponent.CurrentCameraView;
            var inputDomain = battleFacades.Domain.BattleInputDomain;
            Vector2 inputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            inputDomain.UpdateCameraByCameraView(battleFacades.Repo.RoleRepo.Owner, cameraView, currentCam, inputAxis);
        }

        #endregion

    }


}




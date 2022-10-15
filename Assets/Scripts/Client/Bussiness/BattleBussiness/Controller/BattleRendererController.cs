


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
            Vector2 inputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            var inputDomain = battleFacades.Domain.InputDomain;
            inputDomain.UpdateCameraByCameraView(inputAxis);
        }

        #endregion

    }


}




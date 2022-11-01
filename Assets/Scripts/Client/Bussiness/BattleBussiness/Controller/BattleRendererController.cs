


using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattleRendererController
    {

        BattleFacades battleFacades;

        public BattleRendererController()
        {
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick(float fixedDeltaTime)
        {
        }

        public void Update(float deltaTime)
        {
            Update_RoleRenderer(deltaTime);
            Update_BulletRenderer(deltaTime);
            Update_Camera();
        }

        #region [Renderer Update]

        void Update_RoleRenderer(float deltaTime)
        {
            var roleRendererDomain = battleFacades.Domain.RoleRendererDomain;
            roleRendererDomain.Update_WorldUI();
            roleRendererDomain.Update_RoleRenderer(deltaTime);
        }

        void Update_BulletRenderer(float deltaTime)
        {
            var bulletRendererDomain = battleFacades.Domain.BulletRendererDomain;
            bulletRendererDomain.Update_BulletRenderer(deltaTime);
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




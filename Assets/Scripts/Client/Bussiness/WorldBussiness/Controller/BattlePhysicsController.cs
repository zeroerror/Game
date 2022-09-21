using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Protocol.Battle;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.BattleBussiness.Interface;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattlePhysicsController
    {

        BattleFacades battleFacades;
        float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick()
        {
            // == Physics Movement
            if (battleFacades.Repo.FiledRepo.CurFieldEntity != null)
            {
                Tick_Physics_Movement_Role(fixedDeltaTime);
                Tick_Physics_Movement_Bullet(fixedDeltaTime);
            }

            // == Physics Simulation
            var physicsScene = battleFacades.Repo.FiledRepo.CurPhysicsScene;
            physicsScene.Simulate(fixedDeltaTime);

            // == Physics Collision(Only For Client Performances Like Hit Effect,etc.)
            Tick_Physics_Collision_Role(fixedDeltaTime);
            Tick_Physics_Collision_Bullet();

        }

        void Tick_Physics_Collision_Role(float fixedDeltaTime)
        {
            var physicsDomain = battleFacades.Domain.PhysicsDomain;
            var roleList = physicsDomain.Tick_AllRoleHitEnter(fixedDeltaTime);
        }

        void Tick_Physics_Collision_Bullet()
        {
            var physicsDomain = battleFacades.Domain.PhysicsDomain;
            physicsDomain.Refresh_BulletHit();
            // TODO:客户端这边就负责击中特效啥的
        }

        void Tick_Physics_Movement_Role(float deltaTime)
        {
            var domain = battleFacades.Domain.BattleRoleDomain;
            domain.Tick_RoleRigidbody(deltaTime);

            var cameraComponent = battleFacades.Repo.FiledRepo.CurFieldEntity.CameraComponent;
            var currentCamera = cameraComponent.CurrentCamera;
            var cameraView = cameraComponent.CurrentCameraView;
            var inputDomain = battleFacades.Domain.BattleInputDomain;

            Vector2 inputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            inputDomain.UpdateCameraByCameraView(battleFacades.Repo.RoleRepo.Owner, cameraView, currentCamera, inputAxis);
        }

        void Tick_Physics_Movement_Bullet(float fixedDeltaTime)
        {
            var domain = battleFacades.Domain.BulletDomain;
            domain.Tick_Bullet(fixedDeltaTime);
        }

    }


}
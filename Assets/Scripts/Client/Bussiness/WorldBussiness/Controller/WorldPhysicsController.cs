using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.WorldBussiness.Interface;
using Game.Client.Bussiness.WorldBussiness.Generic;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldPhysicsController
    {

        WorldFacades worldFacades;
        float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
        }

        public void Tick()
        {
            // == Physics Movement
            if (worldFacades.Repo.FiledEntityRepo.CurFieldEntity != null)
            {
                Tick_Physics_Movement_Role(fixedDeltaTime);
                Tick_Physics_Movement_Bullet(fixedDeltaTime);
            }

            // == Physics Simulation
            var physicsScene = worldFacades.Repo.FiledEntityRepo.CurPhysicsScene;
            physicsScene.Simulate(fixedDeltaTime);

            // == Physics Collision(Only For Client Performances Like Hit Effect,etc.)
            Tick_Physics_Collision_Role(fixedDeltaTime);
            Tick_Physics_Collision_Bullet();

        }

        void Tick_Physics_Collision_Role(float fixedDeltaTime)
        {
            var physicsDomain = worldFacades.Domain.PhysicsDomain;
            var roleList = physicsDomain.Tick_AllRoleHitEnter(fixedDeltaTime);
        }

        void Tick_Physics_Collision_Bullet()
        {
            var physicsDomain = worldFacades.Domain.PhysicsDomain;
            physicsDomain.Refresh_BulletHit();
            // TODO:客户端这边就负责击中特效啥的
        }

        void Tick_Physics_Movement_Role(float deltaTime)
        {
            var domain = worldFacades.Domain.WorldRoleDomain;
            domain.Tick_RoleRigidbody(deltaTime);

            var cameraComponent = worldFacades.Repo.FiledEntityRepo.CurFieldEntity.CameraComponent;
            var currentCamera = cameraComponent.CurrentCamera;
            var cameraView = cameraComponent.CurrentCameraView;
            var inputDomain = worldFacades.Domain.WorldInputDomain;

            Vector2 inputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            inputDomain.UpdateCameraByCameraView(worldFacades.Repo.WorldRoleRepo.Owner, cameraView, currentCamera, inputAxis);
        }

        void Tick_Physics_Movement_Bullet(float fixedDeltaTime)
        {
            var domain = worldFacades.Domain.BulletDomain;
            domain.Tick_Bullet(fixedDeltaTime);
        }

    }


}
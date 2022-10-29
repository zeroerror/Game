using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Protocol.Battle;
using Game.Client.Bussiness.EventCenter;
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
            Tick_Physics_Collision(fixedDeltaTime);

        }

        void Tick_Physics_Collision(float fixedDeltaTime)
        {
            var physicsDomain = battleFacades.Domain.PhysicsDomain;
            physicsDomain.Tick_RoleHitField();
            physicsDomain.Tick_BulletHitField();
        }

        void Tick_Physics_Movement_Role(float deltaTime)
        {
            var domain = battleFacades.Domain.RoleDomain;
            domain.Tick_RoleRigidbody(deltaTime);

            Vector2 inputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            var inputDomain = battleFacades.Domain.InputDomain;
            inputDomain.UpdateCameraByCameraView(inputAxis);
        }

        void Tick_Physics_Movement_Bullet(float fixedDeltaTime)
        {
            var domain = battleFacades.Domain.BulletDomain;
            domain.Tick_BulletMovement(fixedDeltaTime);
        }

    }


}
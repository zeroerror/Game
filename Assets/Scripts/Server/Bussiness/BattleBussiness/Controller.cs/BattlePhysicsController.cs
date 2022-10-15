using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattlePhysicsController
    {

        BattleServerFacades battleFacades;
        float fixedDeltaTime;

        // 当前所有ConnId
        List<int> connIdList => battleFacades.Network.connIdList;

        public BattlePhysicsController()
        {

        }

        public void Inject(BattleServerFacades battleFacades, float fixedDeltaTime)
        {
            this.battleFacades = battleFacades;
            this.fixedDeltaTime = fixedDeltaTime;
        }

        public void Tick()
        {
            // Physics Simulation
            Tick_Physics_Movement_Bullet(fixedDeltaTime);
            Tick_Physics_Movement_Role(fixedDeltaTime);
            var physicsScene = battleFacades.BattleFacades.Repo.FiledRepo.CurPhysicsScene;
            physicsScene.Simulate(fixedDeltaTime);

            // Physcis Collision
            Tick_Physics_Collision_Role();
        }

        #region [Physics]

        // 地形造成的减速 TODO:滑铲加速
        void Tick_Physics_Collision_Role()
        {
            var physicsDomain = battleFacades.BattleFacades.Domain.PhysicsDomain;
            var roleList = physicsDomain.Tick_AllRoleHitField(fixedDeltaTime);
        }

        void Tick_Physics_Movement_Role(float fixedDeltaTime)
        {
            var physicsDomain = battleFacades.BattleFacades.Domain.PhysicsDomain;
            // physicsDomain.Tick_RoleMoveHitErase();   //Unity's Collision Will Auto Erase 有BUG!!!!!!!!
            var domain = battleFacades.BattleFacades.Domain.RoleDomain;
            domain.Tick_RoleRigidbody(fixedDeltaTime);
        }

        void Tick_Physics_Movement_Bullet(float fixedDeltaTime)
        {
            var domain = battleFacades.BattleFacades.Domain.BulletDomain;
            domain.Tick_BulletMovement(fixedDeltaTime);
        }

        #endregion


    }

}
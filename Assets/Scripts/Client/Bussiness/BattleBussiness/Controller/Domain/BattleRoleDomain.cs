using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Network;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleRoleDomain
    {

        BattleFacades battleFacades;

        byte tempRidIndex;

        public BattleRoleDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        #region [Spawn]

        public BattleRoleLogicEntity SpawnRoleLogic(Transform parent)
        {
            string rolePrefabName = "role_logic";
            if (battleFacades.Assets.BattleRoleAssets.TryGetByName(rolePrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var roleLogic = prefabAsset.GetComponent<BattleRoleLogicEntity>();
                return roleLogic;
            }

            Debug.Log("生成角色失败");
            return null;
        }

        public BattleRoleRendererEntity SpawnRoleRenderer(Transform parent)
        {
            string rolePrefabName = "role_renderer";
            Debug.Log("生成" + rolePrefabName);
            if (battleFacades.Assets.BattleRoleAssets.TryGetByName(rolePrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var roleRenderer = prefabAsset.GetComponentInChildren<BattleRoleRendererEntity>();
                return roleRenderer;
            }

            return null;
        }

        #endregion

        public void Tick_RoleRigidbody(float fixedTime)
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                role.MoveComponent.Tick_Friction(fixedTime);
                role.MoveComponent.Tick_Gravity(fixedTime);
                role.MoveComponent.Tick_Rigidbody(fixedTime);
            });
        }

        public void RoleMoveActivate(BattleRoleLogicEntity role, Vector3 dir)
        {
            role.MoveComponent.ActivateMoveVelocity(dir);
        }

        public bool TryRoleRoll(BattleRoleLogicEntity role, Vector3 dir)
        {
            role.StateComponent.EnterRolling(40);
            return role.MoveComponent.TryRoll(dir);
        }

        public void RoleReborn(BattleRoleLogicEntity role)
        {
            role.TearDown();
            role.Reborn(battleFacades.Repo.FiledRepo.CurFieldEntity.BornPos);
        }

        public void RoleStateEnterReloading(BattleRoleLogicEntity role)
        {
            role.StateComponent.EnterReloading(40);
        }

         public void RoleStateEnterDead(BattleRoleLogicEntity role)
        {
            role.StateComponent.EnterDead(60);
        }

    }

}
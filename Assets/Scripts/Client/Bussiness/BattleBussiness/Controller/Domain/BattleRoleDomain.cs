using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Network;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleRoleDomain
    {

        BattleFacades battleFacades;

        BattleRoleReqAndRes battleRoleReqAndRes;

        byte tempRidIndex;

        public BattleRoleDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleRoleLogicEntity SpawnBattleRoleLogic(Transform parent)
        {
            string rolePrefabName = "PlayerLogic";
            Debug.Log("生成" + rolePrefabName);
            if (battleFacades.Assets.BattleRoleAssets.TryGetByName(rolePrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var roleLogic = prefabAsset.GetComponent<BattleRoleLogicEntity>();
                return roleLogic;
            }

            return null;
        }

        public BattleRoleRendererEntity SpawnBattleRoleRenderer(Transform parent)
        {
            string rolePrefabName = "PlayerRenderer";
            Debug.Log("生成" + rolePrefabName);
            if (battleFacades.Assets.BattleRoleAssets.TryGetByName(rolePrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var roleRenderer = prefabAsset.GetComponent<BattleRoleRendererEntity>();
                return roleRenderer;
            }

            return null;
        }

        public void Tick_RoleRigidbody(float fixedTime)
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                role.MoveComponent.Tick_Friction(fixedTime);
                role.MoveComponent.Tick_GravityVelocity(fixedTime);
                role.MoveComponent.Tick_Rigidbody(fixedTime);
            });
        }

        public void Update_RoleRenderer(float deltaTime)
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                var renderer = role.roleRenderer;
                renderer.transform.position = Vector3.Lerp(renderer.transform.position, role.MoveComponent.CurPos, deltaTime * renderer.posAdjust);
                renderer.transform.rotation = Quaternion.Lerp(renderer.transform.rotation, role.MoveComponent.Rotation, deltaTime * renderer.rotAdjust);

            });
        }


    }

}
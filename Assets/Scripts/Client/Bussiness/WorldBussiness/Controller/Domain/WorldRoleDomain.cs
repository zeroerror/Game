using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Network;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class WorldRoleDomain
    {

        WorldFacades worldFacades;

        WorldRoleReqAndRes _worldRoleReqAndRes;

        byte tempRidIndex;

        public WorldRoleDomain()
        {
        }

        public void Inject(WorldFacades facades)
        {
            this.worldFacades = facades;
        }

        public WorldRoleLogicEntity SpawnWorldRoleLogic(Transform parent)
        {
            string rolePrefabName = "PlayerLogic";
            Debug.Log("生成" + rolePrefabName);
            if (worldFacades.Assets.WorldRoleAssets.TryGetByName(rolePrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var roleLogic = prefabAsset.GetComponent<WorldRoleLogicEntity>();
                return roleLogic;
            }

            return null;
        }

        public WorldRoleRendererEntity SpawnWorldRoleRenderer(Transform parent)
        {
            string rolePrefabName = "PlayerRenderer";
            Debug.Log("生成" + rolePrefabName);
            if (worldFacades.Assets.WorldRoleAssets.TryGetByName(rolePrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var roleRenderer = prefabAsset.GetComponent<WorldRoleRendererEntity>();
                return roleRenderer;
            }

            return null;
        }

        public void Tick_RoleRigidbody(float fixedTime)
        {
            var roleRepo = worldFacades.Repo.WorldRoleRepo;
            roleRepo.Foreach((role) =>
            {
                role.MoveComponent.Tick_Friction(fixedTime);
                role.MoveComponent.Tick_GravityVelocity(fixedTime);
                role.MoveComponent.Tick_Rigidbody(fixedTime);
            });
        }

        public void Update_RoleRenderer(float deltaTime)
        {
            var roleRepo = worldFacades.Repo.WorldRoleRepo;
            roleRepo.Foreach((role) =>
            {
                var renderer = role.roleRenderer;
                renderer.transform.position = Vector3.Lerp(renderer.transform.position, role.MoveComponent.CurPos, deltaTime * renderer.posAdjust);
                renderer.transform.rotation = Quaternion.Lerp(renderer.transform.rotation, role.MoveComponent.Rotation, deltaTime * renderer.rotAdjust);

            });
        }


    }

}
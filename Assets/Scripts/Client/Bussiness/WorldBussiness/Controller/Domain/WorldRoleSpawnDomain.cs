using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Network;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class WorldRoleSpawnDomain
    {

        WorldFacades worldFacades;

        WorldRoleReqAndRes _worldRoleReqAndRes;

        byte tempRidIndex;

        public WorldRoleSpawnDomain()
        {
        }

        public void Inject(WorldFacades facades)
        {
            this.worldFacades = facades;
        }


        public WorldRoleEntity SpawnWorldRole(Transform parent)
        {
            string rolePrefabName = "Player";
            Debug.Log("生成" + rolePrefabName);
            if (worldFacades.Assets.WorldRoleAssets.TryGetByName(rolePrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var entity = prefabAsset.GetComponent<WorldRoleEntity>();
                entity.Ctor();
                entity.SetWRid(++tempRidIndex);

                return entity;
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

        public void Tick_RoleCameraTracking()
        {
            var owner = worldFacades.Repo.WorldRoleRepo.Owner;
            if (owner == null) return;
            var trackPos = owner.MoveComponent.CurPos + owner.transform.forward * 0.5f;
            trackPos.y -= 1f;
            owner.SetCamTrackingPos(trackPos);
        }

    }

}
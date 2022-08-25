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
                entity.SetWRid(++tempRidIndex);

                return entity;
            }

            return null;

        }

    }

}